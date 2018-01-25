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

            var repotsSettings = Uow.ReportsSettingsRepository.GetQueryByMemberIdAsNoTrakingWithIncludes(member.Id);

            var newReportsSettings = new ReportsSettings
            {
                Id = repotsSettings?.Id ?? 0,

                MemberId = member.Id,
                GroupById = reportsSettings.GroupById ?? (int)Constants.ReportsGroupBy.Date,
                DateFrom = reportsSettings.DateFrom,
                DateTo = reportsSettings.DateTo,
                ProjectIds = reportsSettings.ProjectIds != null ? string.Join(",", reportsSettings.ProjectIds) : null,
                MemberIds = reportsSettings.MemberIds != null ? string.Join(",", reportsSettings.MemberIds) : null,
                ClientIds = reportsSettings.ClientIds != null ? string.Join(",", reportsSettings.ClientIds) : null,
                ShowColumnIds = reportsSettings.ShowColumnIds != null ? string.Join(",", reportsSettings.ShowColumnIds) : null
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
    }
}