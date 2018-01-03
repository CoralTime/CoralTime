using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Errors;
using System.Collections.Generic;

namespace CoralTime.BL.Helpers
{
    public partial class BLHelpers
    {
        public static void CheckClientsErrors(Client clientData)
        {
            var errors = new List<ErrorView>();

            if (string.IsNullOrEmpty(clientData.Name))
            {
                errors.Add(new ErrorView
                {
                    Source = "Name",
                    Title = "Name is required",
                    Details = "Name is required and should be at least 6 characters long."
                });
            }

            if (errors.Count > 0)
            {
                throw new CoralTimeSafeEntityException()
                {
                    errors = errors
                };
            }
        }

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

        public static void CheckProjectsErrors(Project projectData, bool isNameUnique)
        {
            List<ErrorView> errors = new List<ErrorView>();

            if (string.IsNullOrEmpty(projectData.Name) || !isNameUnique)
            {
                errors.Add(new ErrorView
                {
                    Source = "Name",
                    Title = "Name is invalid",
                    Details = "Name is required and should be unique."
                });
            }

            if (errors.Count > 0)
            {
                throw new CoralTimeSafeEntityException()
                {
                    errors = errors
                };
            }
        }

        public static void CheckSettingsErrors(Setting projectSetting, bool isNameUnique = true)
        {
            List<ErrorView> errors = new List<ErrorView>();

            if ((string.IsNullOrEmpty(projectSetting.Name) || !isNameUnique))
            {
                errors.Add(new ErrorView
                {
                    Source = "Name",
                    Title = "Name is invalid",
                    Details = "Name is required and should be unique."
                });
            }

            if (errors.Count > 0)
            {
                throw new CoralTimeSafeEntityException()
                {
                    errors = errors
                };
            }
        }
    }
}