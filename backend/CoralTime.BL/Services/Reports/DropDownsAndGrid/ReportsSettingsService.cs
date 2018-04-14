using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
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

        public void UpdateCurrentQuery(ReportsSettingsView reportsSettingsView, int memberImpersonatedId)
        {
            var reportsSettings = Uow.ReportsSettingsRepository.GetEntityFromContext_ByMemberIdQueryName(memberImpersonatedId, reportsSettingsView.QueryName);
            if (reportsSettings != null)
            {
                ResetIsCustomQueryForAllQueryThisMember(memberImpersonatedId);

                reportsSettings.GetModel(reportsSettingsView, memberImpersonatedId);
                Uow.ReportsSettingsRepository.Update(reportsSettings);

                Uow.Save();
                Uow.ReportsSettingsRepository.LinkedCacheClear();
            }
            else
            {
                throw new CoralTimeDangerException("You cann't to save unexisted query");
            }
        }

        public void SaveCustomQuery(ReportsSettingsView reportsSettingsView)
        {
            if (!IsDefaultQuery(reportsSettingsView.QueryName))
            {
                var memberId = MemberImpersonated.Id;

                var reportsSettings = Uow.ReportsSettingsRepository.GetEntityFromContext_ByMemberIdQueryName(memberId, reportsSettingsView.QueryName);
                if (reportsSettings == null)
                {
                    ResetIsCustomQueryForAllQueryThisMember(memberId);

                    reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, memberId);
                    Uow.ReportsSettingsRepository.Insert(reportsSettings);

                    Uow.Save();
                    Uow.ReportsSettingsRepository.LinkedCacheClear();
                }
                else
                {
                    throw new CoralTimeDangerException("You try to save existed custom query");
                }
            }
            else
            {
                throw new CoralTimeDangerException("You try to save not custom query");
            }
        }

        public void DeleteCustomQuery(int queryId)
        {
            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntityFromContex_ByMemberIdQueryId(MemberImpersonated.Id, queryId);

            if (getReportsSettingsByid == null)
            {
                throw new CoralTimeEntityNotFoundException($"There is no record for this member by id = {queryId}");
            }

            if (!IsDefaultQuery(getReportsSettingsByid.QueryName))
            {
                Uow.ReportsSettingsRepository.Delete(getReportsSettingsByid);
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

        private void ResetIsCustomQueryForAllQueryThisMember(int memberId)
        {
            var allQueries = Uow.ReportsSettingsRepository.GetEntitiesFromContex_ByMemberId(memberId);
            if (allQueries != null && allQueries.Count > 0)
            {
                allQueries.ForEach(query => query.IsCurrentQuery = false);
                Uow.ReportsSettingsRepository.UpdateRange(allQueries);
            }
        }      
    }
}
