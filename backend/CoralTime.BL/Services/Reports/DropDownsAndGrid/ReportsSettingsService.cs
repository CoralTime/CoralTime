using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertViewToModel;
using CoralTime.DAL.Models.ReportsSettings;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Reports.Request.ReportsSettingsView;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public class ReportsSettingsService : BaseService, IReportsSettingsService
    {
        public ReportsSettingsService(UnitOfWork uow, IMapper mapper)
            : base(uow, mapper) { }

        public void UpdateCurrentQuery(ReportsSettingsView reportsSettingsView)
        {
            var reportsSettings = Uow.ReportsSettingsRepository.GetQueryByMemberIdQueryName(BaseMemberImpersonated.Id, reportsSettingsView.QueryName);
            if (reportsSettings != null)
            {
                ResetIsCustomQueryForAllQueryThisMember();

                reportsSettings.GetModel(reportsSettingsView, BaseMemberImpersonated.Id);

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
                var reportsSettings = Uow.ReportsSettingsRepository.GetQueryByMemberIdQueryName(BaseMemberImpersonated.Id, reportsSettingsView.QueryName);
                if (reportsSettings == null)
                {
                    ResetIsCustomQueryForAllQueryThisMember();

                    reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, BaseMemberImpersonated.Id);

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
            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetQueryByMemberIdQueryId(BaseMemberImpersonated.Id, queryId);

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

        private bool IsDefaultQuery(string queryName) => string.IsNullOrEmpty(queryName);

        private void ResetIsCustomQueryForAllQueryThisMember()
        {
            var allQueries = Uow.ReportsSettingsRepository.GetQueryByMemberId(BaseMemberImpersonated.Id);
            if (allQueries != null && allQueries.Count > 0)
            {
                allQueries.ForEach(query => query.IsCurrentQuery = false);
                Uow.ReportsSettingsRepository.UpdateRange(allQueries);
            }
        }      
    }
}
