using CoralTime.DAL.Models;
using CoralTime.ViewModels.Azure;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace CoralTime.Services
{
    public class AzureGrant : IExtensionGrantValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AzureGrant> _logger;
        private readonly IMemoryCache _memoryCache;

        public AzureGrant(
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            ILogger<AzureGrant> logger,
            IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public string GrantType
        {
            get
            {
                return "azureAuth";
            }
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userToken = context.Request.Raw.Get("id_token");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenErrors.InvalidGrant, null);
                return;
            }

            try
            {
                var token = await ValidateTokenAsync(userToken);
                if (token == null)
                {
                    context.Result = new GrantValidationResult(TokenErrors.InvalidGrant, null);
                    return;
                }

                var userName = token.Claims.FirstOrDefault(m => m.Type == "unique_name")?.Value;

                // get user's identity
                var user = await _userManager.FindByNameAsync(userName);

                if (user != null && ((user?.IsActive) ?? false))
                {
                    context.Result = new GrantValidationResult(user.Id, "azure");
                    return;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                context.Result = new GrantValidationResult(TokenErrors.InvalidGrant, null);
                return;
            }
        }

        private async Task<JwtSecurityToken> ValidateTokenAsync(string jwtToken)
        {
            JwtSecurityToken token;
            try
            {
                var certificates = await GetCertificateKeysAsync();
                var tokenToCheck = new JwtSecurityToken(jwtToken);
                var x5t = tokenToCheck.Header.X5t;
                var x509data = Encoding.ASCII.GetBytes(certificates.Keys.FirstOrDefault(x => x.X5t == x5t).X5c.FirstOrDefault());
                var certificate = new X509SecurityKey(new X509Certificate2(x509data));

                var azureIssuer = _config["Authentication:AzureAd:Issuer"];
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = azureIssuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = certificate,
                    ValidateAudience = true,
                    ValidAudience = _config["Authentication:AzureAd:Audience"]
            };

                var jwtHandler = new JwtSecurityTokenHandler();
                jwtHandler.ValidateToken(jwtToken, tokenValidationParameters, out SecurityToken securityToken);
                token = securityToken as JwtSecurityToken;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Azure Token validation: {ex.Message}");
                return null;
            }

            return token;
        }

        private async Task<CertificateKeys> GetCertificateKeysAsync()
        {
            var result = _memoryCache.TryGetValue("CertificateKeys", out CertificateKeys certificates);
            var timeResult = _memoryCache.TryGetValue("CertificateKeysTime", out DateTime certificatesTime);
            if (!result || !timeResult || DateTime.Now.Subtract(certificatesTime).TotalHours >= 24)
            {
                var url = _config["Authentication:AzureAd:CertificatesUrl"];
                var client = new HttpClient();
                var json = await client.GetStringAsync(url);
                certificates = JsonConvert.DeserializeObject<CertificateKeys>(json);
                _memoryCache.Set("CertificateKeysTime", DateTime.Now);
                _memoryCache.Set("CertificateKeys", certificates);
            }

            return certificates;
        }
    }
}