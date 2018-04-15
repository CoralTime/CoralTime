using System;
using System.Collections.Generic;
using CoralTime.Common.Exceptions;
using CoralTime.ViewModels.Errors;

namespace CoralTime.Common.Middlewares
{
    public class ExceptionsChecker
    {
        private static List<ErrorView> CheckException(Exception e)
        {
            if (e is CoralTimeEntityNotFoundException)
            {
                return new List<ErrorView>
                {
                    new ErrorView
                    {
                        Source = "Other",
                        Title = "Entity not found.",
                        Details = e.Message
                    }
                };
            }

            if (e is CoralTimeSafeEntityException ex)
            {
                return ex.errors;
            }

            return new List<ErrorView>
            {
                new ErrorView
                {
                    Source = "Other",
                    Title = "",
                    Details = e.Message
                }
            };
        }

        public static List<ErrorView> CheckMembersException(Exception e)
        {
            if (e is CoralTimeAlreadyExistsException)
            {
                return new List<ErrorView>
                {
                    new ErrorView
                    {
                        Source = "Username",
                        Title = "User with this name already exist.",
                        Details = "Username must be unique for each user."
                    }
                };
            }

            if (e is CoralTimeIncorrectPasswordException)
            {
                var ex = (CoralTimeIncorrectPasswordException)e;
                return ex.errors;
            }

            return CheckException(e);
        }

        public static List<ErrorView> CheckProjectRolesException(Exception e)
        {
            if (e is CoralTimeAlreadyExistsException)
            {
                return new List<ErrorView>
                {
                    new ErrorView
                    {
                        Source = "Other",
                        Title = "Project role already exists.",
                        Details = e.Message
                    }
                };
            }

            return CheckException(e);
        }
    }
}