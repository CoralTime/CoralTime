using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoralTime.Services
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public IdentityWithAdditionalClaimsProfileService(
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IConfiguration config)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _config = config;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(sub);

            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).Distinct().ToList();

            claims.Add(new Claim(type: Constants.JwtIsManagerClaimType, value: user.IsManager.ToString().ToLower()));
            claims.Add(new Claim(type: Constants.JwtRefreshTokenLifeTimeClaimType, value: _config["SlidingRefreshTokenLifetime"]));

            var resultClaims = new List<Claim>();
            foreach (var claim in claims)
            {
                if (resultClaims.FirstOrDefault(x => x.Type == claim.Type && x.Value == claim.Value) == null)
                {
                    resultClaims.Add(claim);
                }
            }
            context.IssuedClaims = resultClaims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(sub);
            if (user != null)
            {
                if (user.IsActive)
                {
                    context.IsActive = user.IsActive;
                }
            }
        }
    }
}