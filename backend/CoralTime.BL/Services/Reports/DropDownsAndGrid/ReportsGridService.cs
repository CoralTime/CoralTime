using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportsService
    {
        #region Get DropDowns and Grid. Filtration By / Grouping By: Projects, Users, Dates, Clients.

        public ReportTotalView GetReportsGrid(ReportsGridView reportsGridView, Member memberFromNotification = null)
        {
            if (memberFromNotification != null)
            {
                UpdateReportMembers(memberFromNotification);
            }

            var reportTotalView = InitializeReportTotalView(reportsGridView);
            var filteredTimeEntries = GetFilteredTimeEntries(reportsGridView);
            if (filteredTimeEntries.Any())
            {
                switch (reportsGridView.CurrentQuery.GroupById)
                {
                    case (int) ReportsGroupByIds.Project:
                    {
                        var timeEntriesGroupByProjects = filteredTimeEntries
                            .GroupBy(i => i.Project)
                            .OrderBy(x => x.Key.Name)
                            .ToDictionary(key => key.Key, value => value.OrderBy(x => x.Date).ToList());

                        reportTotalView = reportTotalView.GetView(timeEntriesGroupByProjects);
                        break;
                    }

                    case (int) ReportsGroupByIds.User:
                    {
                        var timeEntriesGroupByMembers = filteredTimeEntries
                            .GroupBy(i => i.Member)
                            .OrderBy(x => x.Key.FullName)
                            .ToDictionary(key => key.Key, value => value.OrderBy(x => x.Date).ToList());

                        reportTotalView =  reportTotalView.GetView(timeEntriesGroupByMembers);
                        break;
                    }

                    case (int) ReportsGroupByIds.Date:
                    {
                        var timeEntriesGroupByDate = filteredTimeEntries
                            .GroupBy(i => i.Date)
                            .OrderBy(x => x.Key)
                            .ToDictionary(key => key.Key, key => key.OrderBy(x => x.Date).ToList());

                        reportTotalView =  reportTotalView.GetView(timeEntriesGroupByDate);
                        break;
                    }

                    case (int) ReportsGroupByIds.Client:
                    {
                        var timeEntriesGroupByClients = filteredTimeEntries
                            .GroupBy(i => i.Project.Client ?? CreateWithOutClientInstance())
                            .OrderBy(x => x.Key.Name)
                            .ToDictionary(key => key.Key, value => value.OrderBy(x => x.Date).ToList());

                        reportTotalView =  reportTotalView.GetView(timeEntriesGroupByClients);
                        break;
                    }
                }
            }

            SetupReport(reportTotalView, (ReportsGroupByIds)reportsGridView.CurrentQuery.GroupById);
            return reportTotalView;
        }

        private void SetupReport(ReportTotalView report, ReportsGroupByIds reportsGroup)
        {
            report.Items = null;
            report.GroupByType = null;
            if (reportsGroup == ReportsGroupByIds.User)
            {
                foreach (var item in report.GroupedItems)
                {
                    item.GroupByType.MemberUrlIcon = GetMemberIcon(item.GroupByType.MemberId);
                    item.GroupByType.WorkingHoursPerDay = Uow.MemberRepository.LinkedCacheGetById(item.GroupByType.MemberId)?.WorkingHoursPerDay;
                }
            }
            else
            {
                AddMemberIcons(report);
            }
        }

        #region Add member icons in report

        private Dictionary<int, string> iconUrls = new Dictionary<int, string>();

        private string GetMemberIcon(int memberId)
        {
            if (!iconUrls.ContainsKey(memberId))
            {
                var iconUrl = _imageService.GetUrlIcon(memberId);
                iconUrls.Add(memberId, iconUrl);
                return iconUrl;
            }
            return iconUrls[memberId];
        }

        private void AddMemberIcons(ReportTotalView report)
        {
            report.MemberUrlIcon = GetMemberIcon(report.MemberId);
            foreach (var item in report.GroupedItems)
            {
                item.MemberUrlIcon = GetMemberIcon(item.MemberId);
                foreach (var entryItem in item.Items)
                {
                    entryItem.MemberUrlIcon = GetMemberIcon(entryItem.MemberId);
                }
            }
        }

        #endregion Add member icons in report

        public ReportTotalView InitializeReportTotalView(ReportsGridView reportsGridView)
        {
            var dateFrom = reportsGridView.CurrentQuery.DateFrom;
            var dateTo = reportsGridView.CurrentQuery.DateTo;

            var groupById = reportsGridView.CurrentQuery.GroupById;
            var showColumnIds = reportsGridView.CurrentQuery.ShowColumnIds;

            var dateFormatId = reportsGridView.DateFormatId;

            var reportTotalView = new ReportTotalView(groupById, showColumnIds, dateFormatId, dateFrom, dateTo);
            return reportTotalView;
        }

        public void CheckAndSaveCurrentQuery(ReportsGridView reportsGridView)
        {
            var queryDateFrom = reportsGridView.CurrentQuery.DateFrom;
            var queryDateTo = reportsGridView.CurrentQuery.DateTo;

            var currentQuery = reportsGridView.CurrentQuery;

            var dateStaticId = reportsGridView.CurrentQuery.DateStaticId;
            if (dateStaticId != null)
            {
                var dateStaticExtend = CreateDateStaticExtend(dateStaticId, reportsGridView.GetTodayDate);
                var calculateByStaticIdDateFrom = dateStaticExtend.DateFrom;
                var calculateByStaticIdDateTo = dateStaticExtend.DateTo;

                if (queryDateFrom != calculateByStaticIdDateFrom && queryDateTo != calculateByStaticIdDateTo)
                {
                    throw new CoralTimeDangerException("DateFrom, DateTo from query not equals new DateFrom, DateTo that calculated by DateStaticId");
                }
            }

            _reportsSettingsService.UpdateCurrentQuery(currentQuery);
        }

        #endregion

        #region Get DropDowns and Grid. Filtration By / Grouping By: Projects, Users, Dates, Clients. (Common methods)

        private List<TimeEntry> GetFilteredTimeEntries(ReportsGridView reportsGridView)
        {
            var dateFrom = new DateTime();
            var dateTo = new DateTime();

            FillDatesByDateStaticOrDateFromTo(reportsGridView, ref dateFrom, ref dateTo);

            // By Dates (default grouping, i.e. "Group by None"; direct order).
            var timeEntriesByDateOfUser = GetTimeEntryByDate(dateFrom, dateTo);

            // By Projects.
            if (reportsGridView.CurrentQuery?.ProjectIds != null && reportsGridView.CurrentQuery.ProjectIds.Length > 0)
            {
                CheckAndSetIfInFilterChooseSingleProject(reportsGridView);

                timeEntriesByDateOfUser = timeEntriesByDateOfUser.Where(x => reportsGridView.CurrentQuery.ProjectIds.Contains(x.ProjectId));
            }

            // By Members.
            if (reportsGridView.CurrentQuery?.MemberIds != null && reportsGridView.CurrentQuery.MemberIds.Length > 0)
            {
                timeEntriesByDateOfUser = timeEntriesByDateOfUser.Where(x => reportsGridView.CurrentQuery.MemberIds.Contains(x.MemberId));
            }

            // By Clients that has Projects.
            if (reportsGridView.CurrentQuery?.ClientIds != null && reportsGridView.CurrentQuery.ClientIds.Length > 0)
            {
                timeEntriesByDateOfUser = timeEntriesByDateOfUser.Where(x => reportsGridView.CurrentQuery.ClientIds.Contains(x.Project.ClientId) || x.Project.ClientId == null && reportsGridView.CurrentQuery.ClientIds.Contains(WithoutClient.Id));
            }

            return timeEntriesByDateOfUser.ToList();
        }

        private void FillDatesByDateStaticOrDateFromTo(ReportsGridView reportsGridView, ref DateTime dateFrom, ref DateTime dateTo)
        {
            var dateStaticId = reportsGridView.CurrentQuery.DateStaticId;
            var isFilledDateStaticIdAndDateFromDateTo = dateStaticId != null && reportsGridView.CurrentQuery?.DateFrom != null && reportsGridView.CurrentQuery?.DateTo != null;
            var isFilledOnlyDateFromDateTo = dateStaticId == null && reportsGridView.CurrentQuery?.DateFrom != null && reportsGridView.CurrentQuery?.DateTo != null;

            if (!isFilledDateStaticIdAndDateFromDateTo && !isFilledOnlyDateFromDateTo)
            {
                throw new CoralTimeDangerException("Wrong input conditional: to get entities by DateStaticId or Date From/To properties.");
            }
            
            if (isFilledDateStaticIdAndDateFromDateTo)
            {
                var dateStaticExtend = CreateDateStaticExtend(dateStaticId, reportsGridView.GetTodayDate);

                dateFrom = dateStaticExtend.DateFrom;
                dateTo = dateStaticExtend.DateTo;
            }

            if (isFilledOnlyDateFromDateTo)
            {
                dateFrom = (DateTime) reportsGridView.CurrentQuery?.DateFrom;
                dateTo = (DateTime) reportsGridView.CurrentQuery?.DateTo;
            }
        }

        private void CheckAndSetIfInFilterChooseSingleProject(ReportsGridView reportsGridData)
        {
            if (reportsGridData.CurrentQuery.ProjectIds.Length == 1)
            {
                var singleFilteredProjectId = reportsGridData.CurrentQuery.ProjectIds.FirstOrDefault();
                SingleFilteredProjectName = Uow.ProjectRepository.LinkedCacheGetById(singleFilteredProjectId).Name;
            }
        }

        private IQueryable<TimeEntry> GetTimeEntryByDate(DateTime dateFrom, DateTime dateTo)
        {
            // #0 Get timeEntriesByDate.s
            var timeEntriesByDate = Uow.TimeEntryRepository.GetQuery()
                .Include(x => x.Project).ThenInclude(x => x.Client)
                .Include(x => x.Member.User)
                .Include(x => x.TaskType)
                .Where(t => t.Date.Date >= dateFrom.Date && t.Date.Date <= dateTo.Date && t.TimeTimerStart <= 0); // TODO update logic to set values for start/stop timer in TimeEntry create method!

            #region Constrain for Admin: return all TimeEntries.

            if (ReportMemberImpersonated.User.IsAdmin)
            {
                return timeEntriesByDate;
            }

            #endregion

            #region Constrain for Member. return only TimeEntries that manager is assign.

            if (!ReportMemberImpersonated.User.IsAdmin && !ReportMemberImpersonated.User.IsManager)
            {
                // #1. TimeEntries. Get tEntries for this member.
                timeEntriesByDate = timeEntriesByDate.Where(t => t.MemberId == ReportMemberImpersonated.Id);
            }

            #endregion

            #region Constrain for Manager : return #1 TimeEntries that currentMember is assign, #2 TimeEntries for not assign users at Projects (but TEntries was saved), #4 TimeEntries with global projects that not contains in result.

            if (!ReportMemberImpersonated.User.IsAdmin && ReportMemberImpersonated.User.IsManager)
            {
                var managerProjectIds = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                    .Where(r => r.MemberId == ReportMemberImpersonated.Id && r.RoleId == Uow.ProjectRoleRepository.GetManagerRoleId())
                    .Select(x => x.ProjectId)
                    .ToArray();

                // #1. TimeEntries. Get tEntries for this member and tEntries that is current member is Manager!.
                timeEntriesByDate = timeEntriesByDate.Where(t => t.MemberId == ReportMemberImpersonated.Id || managerProjectIds.Contains(t.ProjectId));
            }

            return timeEntriesByDate;

            #endregion
        }

        private Client CreateWithOutClientInstance()
        {
            return new Client
            {
                Id = WithoutClient.Id,
                Name = WithoutClient.Name,
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now,
            };
        }

        #endregion
    }
}