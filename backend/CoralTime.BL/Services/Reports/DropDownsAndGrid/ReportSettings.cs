using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertersViews.ExstensionsMethods;
using CoralTime.ViewModels.Reports.Request.Grid;
using System;
using System.Linq;
using ReportsSettings = CoralTime.DAL.Models.ReportsSettings;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportService
    {
        public void SaveReportsSettings(ReportsSettingsView reportsSettingsView, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(userName).Id;

            var isDefaultQuery = IsDefaultQuery(reportsSettingsView.QueryName);

            var defaultReportsSettings = SetForReportsSettingsFromDbValuesOfView(reportsSettingsView, memberId, true);
            InsertOrUpdateReportsSettings(defaultReportsSettings, isInsertable: true, isUpdatable: true);

            if (!isDefaultQuery)
            {
                var customReportsSettings = SetForReportsSettingsFromDbValuesOfView(reportsSettingsView, memberId, false);
                InsertOrUpdateReportsSettings(customReportsSettings, isInsertable: true, isUpdatable: false);
            }
        }

        public void InsertOrUpdateReportsSettings(ReportsSettings reportsSettings,  bool isInsertable, bool isUpdatable)
        {
            try
            {
                if (reportsSettings.Id == 0)
                {
                    if (isInsertable)
                    {
                        Uow.ReportsSettingsRepository.Insert(reportsSettings);
                    }
                }
                else
                {
                    if (isUpdatable)
                    {
                        Uow.ReportsSettingsRepository.Update(reportsSettings);
                    }
                }

                Uow.Save();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while work with Reports Settings", e);
            }
        }

        private bool IsDefaultQuery(string queryName)
        {
            return string.IsNullOrEmpty(queryName);
        }

        private ReportsSettings SetForReportsSettingsFromDbValuesOfView(ReportsSettingsView reportsSettingsView, int memberId, bool isDefaultQuery)
        {
            var reportsSettingsFromDb = GetReportsSettingByMemberIdAndTypeQueryAndQueryName(memberId, isDefaultQuery, reportsSettingsView.QueryName, null);

            var reportsSettings = reportsSettingsView.GetView(reportsSettingsFromDb, isDefaultQuery, memberId);

            return reportsSettings;
        }

        public void UpdateCustomReportsSettings(ReportsSettingsView reportsSettingsView, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(userName).Id;

            var isDefaultQuery = IsDefaultQuery(reportsSettingsView.QueryName);

            if (!isDefaultQuery)
            {
                var customReportsSettings = SetForReportsSettingsFromDbValuesOfView(reportsSettingsView, memberId, false);

                CheckReportsSettingsByIdForThisMember(reportsSettingsView.QueryId, customReportsSettings);

                InsertOrUpdateReportsSettings(customReportsSettings, isInsertable: false, isUpdatable: true);
            }
        }

        public void DeleteCustomReportsSettings(int id, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(userName).Id;

            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntitiesOutOfContextForThisMemberById(id, memberId);

            CheckReportsSettingsByIdForThisMember(id, getReportsSettingsByid);

            var isDefaultQuery = IsDefaultQuery(getReportsSettingsByid.QueryName);
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

        private void CheckReportsSettingsByIdForThisMember(int? id, ReportsSettings reportsSettings)
        {
            if (reportsSettings == null)
            {
                throw new CoralTimeEntityNotFoundException($"There is no record for this member by id = {id}");
            }
        }

        private ReportsSettings GetReportsSettingByMemberIdAndTypeQueryAndQueryName(int memberId, bool isDefaultQuery, string queryName, int? queryId)
        {
            var reportsSettings = Uow.ReportsSettingsRepository.GetEntitiesOutOfContext().Where(x => x.MemberId == memberId && x.IsDefaultQuery == isDefaultQuery);

            if (queryId != null)
            {
                return reportsSettings.FirstOrDefault(x => x.Id == queryId);
            }
            else
            {
                var tempQueryName = isDefaultQuery ? null : queryName;

                return reportsSettings.FirstOrDefault(x => x.QueryName == tempQueryName);
            }
        }
    }
}
