using CoralTime.DAL.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoralTime.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //repository to get user from db
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IConfiguration _configuration;

        public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        //this is used to validate your user account with provided grant at /connect/token
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                //get your user model from db (by username - in my case its email)
                var user = await _userManager.FindByNameAsync(context.UserName);
                if (user != null)
                {
                    var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, context.Password);

                    //check if password match - remember to hash password if stored as hash in db
                    if (isPasswordCorrect && user.IsActive)
                    {
                        //set the result
                        context.Result = new GrantValidationResult(
                            subject: user.Id.ToString(),
                            authenticationMethod: "custom",
                            claims: GetUserClaims(user));

                        return;
                    }

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, user.IsActive ? "Incorrect password" : "Your account has been deactivated");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist");
                return;
            }
            catch (Exception)
            {
                //TODO add logging
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Internal error");
            }
        }

        //build claims array from user data
        public static Claim[] GetUserClaims(ApplicationUser user)
        {
            return new Claim[]
            {
            new Claim("user_id", user.Id.ToString() ?? ""),
            new Claim(JwtClaimTypes.Email, user.Email  ?? "")
            };
        }
    }
}