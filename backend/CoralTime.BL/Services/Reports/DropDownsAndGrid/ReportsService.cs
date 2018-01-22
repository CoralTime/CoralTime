using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Models.Reports.Request.Grid;
using CoralTime.DAL.Repositories;
using Microsoft.AspNetCore.Hosting;
using System;
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
            var user = Uow.UserRepository.GetRelatedUserByName(userName);
            var member = Uow.MemberRepository.GetQueryByUserName(userName);

            var newReportsSettings = new ReportsSettings();

            newReportsSettings.MemberId = member.Id;
            newReportsSettings.DateFrom = reportsSettings.DateFrom;
            newReportsSettings.DateTo = reportsSettings.DateTo;
            newReportsSettings.GroupById = reportsSettings.GroupById;

            if (reportsSettings.ProjectIds != null)
            {
                newReportsSettings.ProjectIds = string.Join(",", reportsSettings.ProjectIds);
            }

            if (reportsSettings.MemberIds != null)
            {
                newReportsSettings.MemberIds = string.Join(",", reportsSettings.MemberIds);
            }

            if (reportsSettings.ClientIds != null)
            {
                newReportsSettings.ClientIds = string.Join(",", reportsSettings.ClientIds);
            }

            if (reportsSettings.ShowColumnIds != null)
            {
                newReportsSettings.ShowColumnIds = string.Join(",", reportsSettings.ShowColumnIds);
            }

            try
            {
                var repotsSettings = Uow.ReportsSettingsRepository.GetQueryByMemberIdWithIncludes(member.Id);

                if (repotsSettings == null)
                {
                    Uow.ReportsSettingsRepository.Insert(newReportsSettings);
                }
                else
                {
                    repotsSettings.DateFrom = newReportsSettings.DateFrom;
                    repotsSettings.DateTo = newReportsSettings.DateTo;
                    repotsSettings.GroupById = newReportsSettings.GroupById;
                    repotsSettings.ClientIds = newReportsSettings.ClientIds;
                    repotsSettings.ProjectIds = newReportsSettings.ProjectIds;
                    repotsSettings.MemberIds = newReportsSettings.MemberIds;
                    repotsSettings.ShowColumnIds = newReportsSettings.ShowColumnIds;

                    //var repotsSettings = Mapper.Map<ReportsSettings, ReportsSettings>(newReportsSettings);
                    Uow.ReportsSettingsRepository.Update(repotsSettings);
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