using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertersViews.ExstensionsMethods;
using CoralTime.ViewModels.Reports.Request.Grid;
using System;
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
                var defaultQuery = GetQueryAndSetValuesFromViewForQuery(reportsSettingsView, memberId);
                SaveQueryToReportsSettings(defaultQuery);
            }
        }

        public void SaveCustomQuery(ReportsSettingsView reportsSettingsView, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(userName).Id;

            if (!IsDefaultQuery(reportsSettingsView.QueryName))
            {
                var customQuery = GetQueryAndSetValuesFromViewForQuery(reportsSettingsView, memberId);
                SaveQueryToReportsSettings(customQuery);
            }
        }

        public void DeleteCustomQuery(int id, string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(userName).Id;

            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberidQueryId(id, memberId);

            CheckCustomQueryForThisMember(id, getReportsSettingsByid);

            if (!IsDefaultQuery(getReportsSettingsByid.QueryName))
            {
                try
                {
                    Uow.ReportsSettingsRepository.Delete(id);
                    Uow.Save();
                }
                catch (Exception e)
                {
                    throw new CoralTimeDangerException("An error occurred while work with Reports Settings", e);
                }
            }
            else
            {
                throw new CoralTimeDangerException("You cannot delete default query for ReportsSettings");
            }
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

        private void CheckCustomQueryForThisMember(int? id, ReportsSettings reportsSettings)
        {
            if (reportsSettings == null)
            {
                throw new CoralTimeEntityNotFoundException($"There is no record for this member by id = {id}");
            }
        }

        private void IsCustomQueryResetForAllQueries(int memberId)
        {
            var allQueries = Uow.ReportsSettingsRepository.GetEntitiesFromContex_ByMemberid(memberId);
            if (allQueries != null)
            {
                foreach(var query in allQueries)
                {
                    query.IsCurrentQuery = false;
                }

                try
                {
                    Uow.ReportsSettingsRepository.UpdateRange(allQueries);
                    Uow.Save();
                }
                catch (Exception e)
                {
                    throw new CoralTimeDangerException("An error occurred while work with Reports Settings", e);
                }
            }
        }

        private void IsCustomQuerySetForThisQuery(ReportsSettings reportsSettings)
        {
            reportsSettings.IsCurrentQuery = true;
        }

        private bool IsDefaultQuery(string queryName)
        {
            return string.IsNullOrEmpty(queryName);
        }

        private ReportsSettings GetQueryAndSetValuesFromViewForQuery(ReportsSettingsView reportsSettingsView, int memberId)
        {
            IsCustomQueryResetForAllQueries(memberId);

            var queryFromDb = Uow.ReportsSettingsRepository.GetEntityFromContex_ByMemberidQueryname(memberId, reportsSettingsView.QueryName);

            if (queryFromDb == null)
            {
                queryFromDb = reportsSettingsView.GetViewInsert(queryFromDb, memberId);
            }
            else
            {
                queryFromDb = reportsSettingsView.GetViewUpdate(queryFromDb, memberId);
            }

            IsCustomQuerySetForThisQuery(queryFromDb);

            return queryFromDb;
        }
    }
}
