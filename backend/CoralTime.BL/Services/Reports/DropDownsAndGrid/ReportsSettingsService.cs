using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.ConvertViewToModel;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Reports.Request.Grid;
using System.Linq;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public class ReportsSettingsService : BaseService, IReportsSettingsService
    {
        public ReportsSettingsService(UnitOfWork uow, IMapper mapper)
            : base(uow, mapper) { }

        public ReportsSettingsView GetCurrentOrCreateDefaultQuery()
        {
            var memberImpersonatedId = MemberImpersonated.Id;
            var reportsSettings = Uow.ReportsSettingsRepository.LinkedCacheGetByMemberId(memberImpersonatedId).FirstOrDefault(x => x.IsCurrentQuery);

            ReportsSettingsView reportsSettingsView;

            if (reportsSettings == null)
            {
                reportsSettingsView = new ReportsSettingsView().GetViewWithDefaultValues();
                reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, memberImpersonatedId);

                Uow.ReportsSettingsRepository.Insert(reportsSettings);
                Uow.Save();
                Uow.ReportsSettingsRepository.LinkedCacheClear();
            }

            reportsSettingsView = reportsSettings.GetView();

            return reportsSettingsView;
        }

        public ReportsSettingsView SaveCurrentQuery(ReportsSettingsView reportsSettingsView)
        {
            return SaveQuery(reportsSettingsView, MemberImpersonated.Id);
        }

        public void SaveCustomQuery(ReportsSettingsView reportsSettingsView)
        {
            if (!IsDefaultQuery(reportsSettingsView.QueryName))
            {
                SaveQuery(reportsSettingsView, MemberImpersonated.Id);
            }
        }

        public void DeleteCustomQuery(int queryId)
        {
            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberIdQueryId(MemberImpersonated.Id, queryId);

            CheckCustomQueryForThisMember(queryId, getReportsSettingsByid);

            if (!IsDefaultQuery(getReportsSettingsByid.QueryName))
            {
                Uow.ReportsSettingsRepository.Delete(queryId);
                Uow.Save();

                Uow.ReportsSettingsRepository.LinkedCacheClear();
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

        private ReportsSettingsView SaveQuery(ReportsSettingsView reportsSettingsView, int memberId)
        {
            // ResetIsCustomQueryForAllQueries(memberId);
            var allQueries = Uow.ReportsSettingsRepository.GetEntitiesFromContex_ByMemberId(memberId);
            if (allQueries != null && allQueries.Count > 0)
            {
                allQueries.ForEach(query => query.IsCurrentQuery = false);
                Uow.ReportsSettingsRepository.UpdateRange(allQueries);
            }

            // InsertOrUpdateCurrentQueryToDb(reportsSettingsView, memberId);
            var reportsSettings = Uow.ReportsSettingsRepository.GetEntityFromContext_ByMemberIdQueryName(memberId, reportsSettingsView.QueryName);
            if (reportsSettings == null)
            {
                reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, memberId);
                Uow.ReportsSettingsRepository.Insert(reportsSettings);
            }
            else // For save custom qry
            {
                reportsSettings.GetModel(reportsSettingsView, memberId);
                Uow.ReportsSettingsRepository.Update(reportsSettings);
            }

            Uow.Save();
            Uow.ReportsSettingsRepository.LinkedCacheClear();

            reportsSettingsView = reportsSettings.GetView();
            return reportsSettingsView;
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
