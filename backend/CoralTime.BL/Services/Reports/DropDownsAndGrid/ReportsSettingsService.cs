using System.Linq;
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

        public void SaveCurrentQuery(ReportsSettingsView reportsSettingsView)
        {
            var memberId = MemberImpersonated.Id;

            if (IsDefaultQuery(reportsSettingsView.QueryName))
            {
                SaveQuery(reportsSettingsView, memberId);
            }
            else
            {
                // Save Custom qry with changed values
                var allQueries = Uow.ReportsSettingsRepository.GetEntitiesFromContex_ByMemberId(memberId);
                if (allQueries != null && allQueries.Count > 0)
                {
                    allQueries.ForEach(query => query.IsCurrentQuery = false);

                    Uow.ReportsSettingsRepository.UpdateRange(allQueries);
                    Uow.Save();

                    // InsertOrUpdateCurrentQueryToDb(reportsSettingsView, memberId);
                    var reportsSettings = Uow.ReportsSettingsRepository.GetEntityFromContext_ByMemberIdQueryName(memberId, reportsSettingsView.QueryName);
                    if (reportsSettings != null)
                    {
                        reportsSettings.GetModel(reportsSettingsView, memberId);

                        Uow.ReportsSettingsRepository.Update(reportsSettings);
                    }
                    else // For save custom qry
                    {
                        reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, memberId);
                        Uow.ReportsSettingsRepository.Insert(reportsSettings);
                    }

                    Uow.Save();
                    Uow.ReportsSettingsRepository.LinkedCacheClear();
                }
            }
        }

        public void SaveCustomQuery(ReportsSettingsView reportsSettingsView)
        {
            var memberId = MemberImpersonated.Id;

            if (!IsDefaultQuery(reportsSettingsView.QueryName))
            {
                SaveQuery(reportsSettingsView, memberId);
            }
        }

        public void DeleteCustomQuery(int queryId)
        {
            Uow.UserRepository.LinkedCacheGetByUserNameAndCheck(ImpersonatedUserName);
            var memberId = Uow.MemberRepository.GetQueryByUserName(ImpersonatedUserName).Id;

            var getReportsSettingsByid = Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberIdQueryId(memberId, queryId);

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

        private void SaveQuery(ReportsSettingsView reportsSettingsView, int memberId)
        {
            //TODO Transaction!!!! set to down
            // ResetIsCustomQueryForAllQueries(memberId);
            var allQueries = Uow.ReportsSettingsRepository.GetEntitiesFromContex_ByMemberId(memberId);
            if (allQueries != null && allQueries.Count > 0)
            {
                allQueries.ForEach(query => query.IsCurrentQuery = false);

                Uow.ReportsSettingsRepository.UpdateRange(allQueries);
                Uow.Save();

                // InsertOrUpdateCurrentQueryToDb(reportsSettingsView, memberId);
                var reportsSettings = Uow.ReportsSettingsRepository.GetEntityFromContext_ByMemberIdQueryName(memberId, reportsSettingsView.QueryName);
                if (reportsSettings != null)
                {
                    reportsSettings.GetModel(reportsSettingsView, memberId);

                    Uow.ReportsSettingsRepository.Update(reportsSettings);
                }
                else // For save custom qry
                {
                    reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, memberId);
                    Uow.ReportsSettingsRepository.Insert(reportsSettings);
                }

                Uow.Save();

                Uow.ReportsSettingsRepository.LinkedCacheClear();
            }
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
