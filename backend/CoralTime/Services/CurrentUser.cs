using System.Linq;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CoralTime.Services
{
    public static class CurrentUser
    {
        public static string GetUserName(this Controller controller)
        {
            return controller.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name).Value;
        }

        public static string GetUserNameWithImpersonation(this Controller controller)
        {
            var isHeaderThere = controller.HttpContext.Request.Headers.TryGetValue(Constants.ImpersonatedUserNameHeader, out var userName);

            var currentUserClaims = controller.User.Claims;

            if (!isHeaderThere)
            {
                var currentUserName = currentUserClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name).Value;
                return currentUserName;
            }

            if (currentUserClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role).Value != Constants.AdminRole)
           {
                throw new CoralTimeForbiddenException($"You can not impersonate user with userName {userName}");
           }

           return userName.ToString();

        }
    }
}
