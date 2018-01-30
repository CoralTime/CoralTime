using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
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

        public void SaveOrUpdateReportsSettingsQuery(ReportsSettingsView reportsSettingsView, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var member = Uow.MemberRepository.GetQueryByUserName(userName);

            var isDefaultQuery = CheckCustomOrDefaultQuery(reportsSettingsView.QueryName);

            CheckEmptyQueryNameForCustomQuery(reportsSettingsView.QueryName, isDefaultQuery);

            var reportsSettingsFromDb = GetReportsSettingsByMemberIdAndQueryName(member.Id, isDefaultQuery, reportsSettingsView.QueryName);

            var newReportsSettings = SetValuesForNewReportsSettings(reportsSettingsView, reportsSettingsFromDb, isDefaultQuery, member);

            try
            {
                if (reportsSettingsFromDb == null)
                {
                    Uow.ReportsSettingsRepository.Insert(newReportsSettings);
                }
                else
                {
                    var updateOnlyDefaultQuery = isDefaultQuery && !reportsSettingsView.IsUpdateCustomQuery;
                    var updateOnlyCustomQuery = isDefaultQuery && reportsSettingsView.IsUpdateCustomQuery;

                    if (updateOnlyDefaultQuery || updateOnlyCustomQuery)
                    {
                        Uow.ReportsSettingsRepository.Update(newReportsSettings);
                    }
                }

                Uow.Save();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while work with Reports Settings", e);
            }
        }

        public void DeleteCustomReportsSettings(int id, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var member = Uow.MemberRepository.GetQueryByUserName(userName);

            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntitiesOutOfContextForThisMemberById(id, member.Id);

            CheckReportsSettingsByIdForThisMember(id, getReportsSettingsByid);

            var isDefaultQuery = CheckCustomOrDefaultQuery(getReportsSettingsByid.QueryName);
            if (!isDefaultQuery)
            {
                Uow.ReportsSettingsRepository.Delete(id);
                Uow.Save();
            }
            else
            {
                throw new CoralTimeDangerException("You cannot delete default query for ReportsSettings");
            }
        }

        private void CheckReportsSettingsByIdForThisMember(int id, ReportsSettings getReportsSettingsByid)
        {
            if (getReportsSettingsByid == null)
            {
                throw new CoralTimeEntityNotFoundException($"There is no record for this member by id = {id}");
            }
        }

        private ReportsSettings GetReportsSettingsByMemberIdAndQueryName(int memberId, bool isDefaultQuery, string queryName)
        {
            return Uow.ReportsSettingsRepository.GetEntitiesOutOfContext().FirstOrDefault(x => x.MemberId == memberId && x.IsDefaultQuery == isDefaultQuery && x.QueryName == queryName);
        }

        private static bool CheckCustomOrDefaultQuery(string queryName)
        {
            return string.IsNullOrEmpty(queryName);
        }

        private void CheckEmptyQueryNameForCustomQuery(string queryName, bool isDefaultQuery)
        {
            if (!isDefaultQuery && queryName == string.Empty)
            {
                throw new CoralTimeUpdateException("Property \"QueryName\" for custom ReportsSettings query can not contain an empty value.");
            }
        }

        private ReportsSettings SetValuesForNewReportsSettings(ReportsSettingsView reportsSettings, ReportsSettings reportsSettingsFromDb, bool isDefaultQuery, Member member)
        {
            return new ReportsSettings
            {
                Id = reportsSettingsFromDb?.Id ?? 0,
                IsDefaultQuery = isDefaultQuery,
                QueryName = reportsSettings.QueryName,

                MemberId = member.Id,
                GroupById = reportsSettings.GroupById ?? (int) Constants.ReportsGroupBy.Date,
                DateFrom = reportsSettings.DateFrom,
                DateTo = reportsSettings.DateTo,
                FilterProjectIds = ConvertFromArrayOfIntsToString(reportsSettings.ProjectIds),
                FilterMemberIds = ConvertFromArrayOfIntsToString(reportsSettings.MemberIds),
                FilterClientIds = ConvertFromArrayOfNullableIntsToString(reportsSettings.ClientIds),
                FilterShowColumnIds = ConvertFromArrayOfIntsToString(reportsSettings.ShowColumnIds)
            };
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