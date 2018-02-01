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
        public void SaveCurrentQuery(ReportsSettingsView reportsSettingsView, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(userName).Id;

            if (IsDefaultQuery(reportsSettingsView.QueryName))
            {
                var defaultQuery = SetValuesForQuery(reportsSettingsView, memberId);
                SaveQueryToReportsSettings(defaultQuery);
            }

            //if (!isDefaultQuery)
            //{
            //    var customReportsSettings = SetForReportsSettingsFromDbValuesOfView(reportsSettingsView, memberId, false);

            //    CheckReportsSettingsByIdForThisMember(reportsSettingsView.QueryId, customReportsSettings);

            //    InsertOrUpdateReportsSettings(customReportsSettings, isInsertable: true, isUpdatable: false);
            //}
        }

        public void SaveCustomQuery(ReportsSettingsView reportsSettings, string userName)
        {

        }

        public void DeleteCustomQuery(int id, string userName)
        {
            //Uow.UserRepository.GetRelatedUserByName(userName);
            //var memberId = Uow.MemberRepository.GetQueryByUserName(userName).Id;

            //var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntitiesOutOfContextForThisMemberById(id, memberId);

            //CheckReportsSettingsByIdForThisMember(id, getReportsSettingsByid);

            //var isDefaultQuery = IsDefaultQuery(getReportsSettingsByid.QueryName);
            //if (!isDefaultQuery)
            //{
            //    Uow.ReportsSettingsRepository.Delete(id);
            //    Uow.Save();
            //}
            //else
            //{
            //    throw new CoralTimeDangerException("You cannot delete default query for ReportsSettings");
            //}
        }

        private void SaveQueryToReportsSettings(ReportsSettings reportsSettings)
        {
            try
            {
                if (reportsSettings.Id == 0)
                {
                    Uow.ReportsSettingsRepository.Insert(reportsSettings);
                }
                else
                {
                    Uow.ReportsSettingsRepository.Update(reportsSettings);
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

        private ReportsSettings SetValuesForQuery(ReportsSettingsView reportsSettingsView, int memberId)
        {
            var queryFromDb = GetQueryFromReportsSettings(memberId, reportsSettingsView.QueryName, reportsSettingsView.QueryId);

            var reportsSettings = reportsSettingsView.GetView(queryFromDb, memberId);

            return reportsSettings;
        }

        private void CheckReportsSettingsByIdForThisMember(int? id, ReportsSettings reportsSettings)
        {
            if (reportsSettings == null)
            {
                throw new CoralTimeEntityNotFoundException($"There is no record for this member by id = {id}");
            }
        }

        private ReportsSettings GetQueryFromReportsSettings(int memberId, string queryName, int? queryId = null)
        {
            if (queryId == null)
            {
                return Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberidQueryname(memberId, queryName);
            }
            else
            {
                return Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberidQuerynameQueryId(memberId, queryName, queryId);
            }
        }
    }
}
