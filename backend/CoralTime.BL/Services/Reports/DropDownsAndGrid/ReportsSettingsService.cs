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

        public ReportsSettingsView GetCurrentOrDefaultQuery()
        {
            var reportsSettings = Uow.ReportsSettingsRepository.LinkedCacheGetByMemberId(MemberImpersonated.Id).FirstOrDefault(x => x.IsCurrentQuery);

            ReportsSettingsView reportsSettingsView;

            if (reportsSettings == null)
            {
                reportsSettingsView = new ReportsSettingsView().GetViewWithDefaultValues();
            }
            else
            {
                reportsSettingsView = reportsSettings.GetView();
            }

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
                var reportsSettings = Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberIdQueryName(memberId, reportsSettingsView.QueryName);
                if(reportsSettings != null && !reportsSettings.IsCurrentQuery)
                {
                    //ResetIsCustomQueryForAllQueries(memberId);
                    var allQueries = Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberId(memberId);
                    if (allQueries != null || allQueries.Count > 0)
                    {
                        allQueries.ForEach(query => query.IsCurrentQuery = false);

                        Uow.ReportsSettingsRepository.UpdateRange(allQueries);
                        //Uow.Save();
                    }

                    reportsSettings.IsCurrentQuery = true;

                    Uow.ReportsSettingsRepository.Update(reportsSettings);
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
            // ResetIsCustomQueryForAllQueries(memberId);
            var allQueries = Uow.ReportsSettingsRepository.GetEntityOutOfContex_ByMemberId(memberId);
            if (allQueries != null && allQueries.Count > 0)
            {
                allQueries.ForEach(query => query.IsCurrentQuery = false);

                Uow.ReportsSettingsRepository.UpdateRange(allQueries);
                //Uow.Save();
            }

            // InsertOrUpdateCurrentQueryToDb(reportsSettingsView, memberId);
            var reportsSettings = Uow.ReportsSettingsRepository.GetEntity_ByMemberIdQueryName(memberId, reportsSettingsView.QueryName);

            if (reportsSettings == null)
            {
                reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, memberId);

                Uow.ReportsSettingsRepository.Insert(reportsSettings);
            }
            else
            {
                reportsSettings.GetModel(reportsSettingsView, memberId);

                Uow.ReportsSettingsRepository.Update(reportsSettings);
            }

            Uow.Save();
            Uow.ReportsSettingsRepository.LinkedCacheClear();
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
