using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Hosting;
using System;
using CoralTime.Common.Constants;
using ReportsSettings = CoralTime.DAL.Models.ReportsSettings;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportService : BaseService, IReportService
    {
        private IHostingEnvironment _environment { get; }

        public ReportService(UnitOfWork uow, IMapper mapper, IHostingEnvironment environment)
            : base(uow, mapper)
        {
            _environment = environment;
        }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public void SaveReportsSettings(RequestReportsSettings reportsSettings, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var member = Uow.MemberRepository.GetQueryByUserName(userName);

            var repotsSettings = Uow.ReportsSettingsRepository.GetQueryAsNoTrakingDefaultByMemberId(member.Id);

            var newReportsSettings = new ReportsSettings
            {
                Id = repotsSettings?.Id ?? 0,
                IsDefaultQuery = true,

                MemberId = member.Id,
                GroupById = reportsSettings.GroupById ?? (int) Constants.ReportsGroupBy.Date,
                DateFrom = reportsSettings.DateFrom,
                DateTo = reportsSettings.DateTo,
                ProjectIds = ConvertFromArrayOfIntsToString(reportsSettings.ProjectIds),
                MemberIds = ConvertFromArrayOfIntsToString(reportsSettings.MemberIds),
                ClientIds = ConvertFromArrayOfNullableIntsToString(reportsSettings.ClientIds),
                ShowColumnIds = ConvertFromArrayOfIntsToString(reportsSettings.ShowColumnIds)
            };

            try
            {
                if (repotsSettings == null)
                {
                    Uow.ReportsSettingsRepository.Insert(newReportsSettings);
                }
                else
                {
                    Uow.ReportsSettingsRepository.Update(newReportsSettings);
                }

                Uow.Save();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while creating new client", e);
            }
        }

        private static string ConvertFromArrayOfIntsToString(int[] array)
        {
            return array != null ? string.Join(",", array) : null;
        }

        private static string ConvertFromArrayOfNullableIntsToString(int?[] array)
        {
            return array != null ? string.Join(",", array) : null;
        }
    }
}