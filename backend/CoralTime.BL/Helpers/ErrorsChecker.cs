using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Errors;
using System.Collections.Generic;

namespace CoralTime.BL.Helpers
{
    public partial class BLHelpers
    {
        public static void CheckClientsErrors(Client clientData)
        {
            var errors = new List<ErrorODataView>();

            if (string.IsNullOrEmpty(clientData.Name))
            {
                errors.Add(new ErrorODataView
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

        public static void CheckProjectsErrors(Project projectData, bool isNameUnique)
        {
            List<ErrorODataView> errors = new List<ErrorODataView>();

            if (string.IsNullOrEmpty(projectData.Name) || !isNameUnique)
            {
                errors.Add(new ErrorODataView
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
            List<ErrorODataView> errors = new List<ErrorODataView>();

            if ((string.IsNullOrEmpty(projectSetting.Name) || !isNameUnique))
            {
                errors.Add(new ErrorODataView
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