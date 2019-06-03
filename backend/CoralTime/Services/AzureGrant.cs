using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Azure;
using IdentityServer4.Models;
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

        public string GrantType => Constants.Authorization.CoralTimeAzure.GrantType;

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userToken = context.Request.Raw.Get(Constants.Authorization.CoralTimeAzure.UserTokenHeader);

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Token is empty");
                return;
            }

            try
            {
                var token = await ValidateTokenAsync(userToken);
                if (token == null)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid token");
                    return;
                }

                var userName = token.Claims.FirstOrDefault(m => m.Type == Constants.Authorization.CoralTimeAzure.UserNameClaim)?.Value;

                var user = await _userManager.FindByNameAsync(userName);

                if (user != null && ((user?.IsActive) ?? false))
                {
                    context.Result = new GrantValidationResult(user.Id, Constants.Authorization.CoralTimeAzure.AuthenticationMethod);
                    return;
                }

                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, null);
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
                jwtHandler.ValidateToken(jwtToken, tokenValidationParameters, out var securityToken);
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
            var certificateKeys = _memoryCache.TryGetValue(Constants.CertificateKeys, out CertificateKeys certificates);
            var certificateKeysTime = _memoryCache.TryGetValue(Constants.CertificateKeysTime, out DateTime certificatesTime);

            if (!certificateKeys || !certificateKeysTime || DateTime.Now.Subtract(certificatesTime).TotalHours >= 24)
            {
                var url = _config["Authentication:AzureAd:CertificatesUrl"];
                var client = new HttpClient();
                var json = await client.GetStringAsync(url);

                certificates = JsonConvert.DeserializeObject<CertificateKeys>(json);
                
                _memoryCache.Set(Constants.CertificateKeysTime, DateTime.Now);
                _memoryCache.Set(Constants.CertificateKeys, certificates);
            }

            return certificates;
        }
    }
}