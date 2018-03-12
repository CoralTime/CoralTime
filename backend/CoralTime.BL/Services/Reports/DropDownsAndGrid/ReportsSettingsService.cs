using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertViewToModel;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public class ReportsSettingsService : BaseService, IReportsSettingsService
    {
        public ReportsSettingsService(UnitOfWork uow, IMapper mapper) 
            : base(uow, mapper) { }

        public void SaveCurrentQuery(ReportsSettingsView reportsSettingsView)
        {
            Uow.UserRepository.GetRelatedUserByName(InpersonatedUserName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(InpersonatedUserName).Id;

            if (IsDefaultQuery(reportsSettingsView.QueryName))
            {
                SaveQuery(reportsSettingsView, memberId);
            }
            else
            {
                var queryFromReportsSettings = Uow.ReportsSettingsRepository.GetEntityFromContex_ByMemberidQueryname(memberId, reportsSettingsView.QueryName);
                if(queryFromReportsSettings != null && !queryFromReportsSettings.IsCurrentQuery)
                {
                    ResetIsCustomQueryForAllQueries(memberId);

                    SetIsCurrentQueryTrue(queryFromReportsSettings);

                    Uow.ReportsSettingsRepository.Update(queryFromReportsSettings);
                    Uow.Save();
                }
            }
        }

        private void SetIsCurrentQueryTrue(ReportsSettings queryFromReportsSettings)
        {
            queryFromReportsSettings.IsCurrentQuery = true;
        }

        public void SaveCustomQuery(ReportsSettingsView reportsSettingsView)
        {
            Uow.UserRepository.GetRelatedUserByName(InpersonatedUserName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(InpersonatedUserName).Id;

            if (!IsDefaultQuery(reportsSettingsView.QueryName))
            {
                SaveQuery(reportsSettingsView, memberId);
            }
        }

        public void DeleteCustomQuery(int id)
        {
            Uow.UserRepository.GetRelatedUserByName(InpersonatedUserName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(InpersonatedUserName).Id;

            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberidQueryId(memberId: memberId, queryId: id);

            CheckCustomQueryForThisMember(id, getReportsSettingsByid);

            if (!IsDefaultQuery(getReportsSettingsByid.QueryName))
            {
                Uow.ReportsSettingsRepository.Delete(id);
                Uow.Save();
            }
            else
            {
                throw new CoralTimeDangerException("You cannot delete default query for ReportsSettings");
            }
        }

        private bool IsDefaultQuery(string queryName)
        {
            return string.IsNullOrEmpty(queryName);
        }

        private void SaveQuery(ReportsSettingsView reportsSettingsView, int memberId)
        {
            ResetIsCustomQueryForAllQueries(memberId);
            SaveQueryToReportsSettings(reportsSettingsView, memberId);
        }

        private void ResetIsCustomQueryForAllQueries(int memberId)
        {
            var allQueries = Uow.ReportsSettingsRepository.GetEntitiesFromContex_ByMemberid(memberId);
            if (allQueries != null)
            {
                allQueries.ForEach(query => query.IsCurrentQuery = false);

                Uow.ReportsSettingsRepository.UpdateRange(allQueries);
                Uow.Save();
            }
        }

        private void SaveQueryToReportsSettings(ReportsSettingsView reportsSettingsView, int memberId)
        {
            var queryFromReportsSettings = Uow.ReportsSettingsRepository.GetEntityFromContex_ByMemberidQueryname(memberId, reportsSettingsView.QueryName);
        
            if (queryFromReportsSettings == null)
            {
                queryFromReportsSettings = new ReportsSettings().GetModel(reportsSettingsView, memberId);

                SetIsCurrentQueryTrue(queryFromReportsSettings);

                Uow.ReportsSettingsRepository.Insert(queryFromReportsSettings);
            }
            else
            {
                queryFromReportsSettings.GetModel(reportsSettingsView, memberId);

                SetIsCurrentQueryTrue(queryFromReportsSettings);

                Uow.ReportsSettingsRepository.Update(queryFromReportsSettings);
            }

            Uow.Save();
        }

        private void CheckCustomQueryForThisMember(int? id, ReportsSettings reportsSettings)
        {
            if (reportsSettings == null)
            {
                throw new CoralTimeEntityNotFoundException($"There is no record for this member by id = {id}");
            }
        }
    }
}
