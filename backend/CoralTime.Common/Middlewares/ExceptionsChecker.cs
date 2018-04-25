using CoralTime.Common.Exceptions;
using CoralTime.ViewModels.Errors;
using System;
using System.Collections.Generic;

namespace CoralTime.Common.Middlewares
{
    public class ExceptionsODataChecker
    {
        public static List<ErrorView> CheckExceptions(Exception exception)
        {
            switch (exception)
            {
                case CoralTimeSafeEntityException ex:
                {
                    return new List<ErrorView>
                    {
                        new ErrorView
                        {
                            Source = "Other",
                            Title = "Safe Entity Exception",
                            Details = ex.Message
                        }
                    };
                }

                case CoralTimeAlreadyExistsException ex:
                {
                    return new List<ErrorView>
                    {
                        new ErrorView
                        {
                            Source = "Other",
                            Title = "Project role already exists.",
                            Details = ex.Message
                        }
                    };
                }

                case CoralTimeEntityNotFoundException ex:
                {
                    return new List<ErrorView>
                    {
                        new ErrorView
                        {
                            Source = "Other",
                            Title = "Entity not found Exception",
                            Details = ex.Message
                        }
                    };
                }

                case CoralTimeDangerException ex:
                {
                    return new List<ErrorView>
                    {
                        new ErrorView
                        {
                            Source = "Other",
                            Title = "Danger Exception.",
                            Details = ex.Message
                        }
                    };
                }

                case CoralTimeIncorrectPasswordException ex:
                {
                    return new List<ErrorView>
                    {
                        new ErrorView
                        {
                            Source = "Other",
                            Title = "Incorrect Password Exception.",
                            Details = ex.Message
                        }
                    };
                }

                case CoralTimeForbiddenException ex:
                {
                    return new List<ErrorView>
                    {
                        new ErrorView
                        {
                            Source = "Other",
                            Title = "Forbidden Exception.",
                            Details = ex.Message
                        }
                    };
                }

                default:
                {
                    return new List<ErrorView>
                    {
                        new ErrorView
                        {
                            Source = "Other",
                            Title = "",
                            Details = exception.Message
                        }
                    };
                }
            }
        }
    }
}