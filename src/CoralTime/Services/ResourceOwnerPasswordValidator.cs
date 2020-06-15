using CoralTime.DAL.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoralTime.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //this is used to validate your user account with provided grant at /connect/token
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(context.UserName);
                if (user != null)
                {
                    var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, context.Password);

                    if (isPasswordCorrect && user.IsActive)
                    {
                        context.Result = new GrantValidationResult(
                            subject: user.Id,
                            authenticationMethod: "custom",
                            claims: GetUserClaims(user));

                        return;
                    }

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, user.IsActive ? "Incorrect password" : "Your account has been deactivated");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist");
            }
            catch (Exception)
            {
                //TODO add logging
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Internal error");
            }
        }

        private static IEnumerable<Claim> GetUserClaims(ApplicationUser user)
        {
            return new[]
            {
                new Claim(type: "user_id", value: user.Id ?? ""),
                new Claim(type: JwtClaimTypes.Email, value: user.Email  ?? "")
            };
        }
    }
}