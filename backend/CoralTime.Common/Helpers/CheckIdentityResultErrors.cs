using CoralTime.ViewModels.Errors;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        public static void CheckIdentityResultErrors(IdentityResult userCreateRoleResult)
        {
            CheckMembersErrors(userCreateRoleResult.Errors.Select(e => new IdentityErrorView
            {
                Code = e.Code,
                Description = e.Description
            }));
        }
    }
}