using CoralTime.Common.Exceptions;
using CoralTime.ViewModels.Errors;
using System.Collections.Generic;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        public static void CheckMembersErrors(IEnumerable<IdentityErrorView> result)
        {
            var passwordErrors = new List<ErrorView>();
            var otherException = new List<ErrorView>();

            foreach (var error in result)
            {
                if (error.Code.Contains("Password"))
                {
                    passwordErrors.Add(new ErrorView
                    {
                        Source = "Password",
                        Title = StringHandler.SeparateStringByUpperCase(error.Code),
                        Details = error.Description
                    });
                }
                else if (error.Code.Contains("UserName"))
                {
                    otherException.Add(new ErrorView
                    {
                        Source = "UserName",
                        Title = StringHandler.SeparateStringByUpperCase(error.Code),
                        Details = error.Description
                    });
                }
                else
                {
                    otherException.Add(new ErrorView
                    {
                        Source = "Other",
                        Title = StringHandler.SeparateStringByUpperCase(error.Code),
                        Details = error.Description
                    });
                }
            }
            if (passwordErrors.Count > 0)
            {
                throw new CoralTimeIncorrectPasswordException
                {
                    errors = passwordErrors
                };
            }
            if (otherException.Count > 0)
            {
                throw new CoralTimeSafeEntityException
                {
                    errors = otherException
                };
            }
        }
    }
}
