using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using IdentityModel;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            var isImpersonatedUser = controller.HttpContext.Request.Headers.TryGetValue(Constants.ImpersonatedUserNameHeader, out var userName);

            var currentUserClaims = controller.User.Claims;

            if (!isImpersonatedUser)
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

        public static string GetUserName(this ODataController controller)
        {
            return controller.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name).Value;
        }

        public static string GetUserNameWithImpersonation(this ODataController controller)
        {
            var isImpersonatedUser = controller.HttpContext.Request.Headers.TryGetValue(Constants.ImpersonatedUserNameHeader, out var userName);

            var currentUserClaims = controller.User.Claims;

            if (!isImpersonatedUser)
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