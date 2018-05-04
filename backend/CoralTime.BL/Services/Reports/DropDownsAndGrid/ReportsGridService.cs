using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportsService
    {
        #region Get DropDowns and Grid. Filtration By / Grouping By: Projects, Users, Dates, Clients.

        public ReportTotalView GetReportsGrid(ReportsGridView reportsGridView)
        {
            var dateFrom = reportsGridView.CurrentQuery.DateFrom;
            var dateTo = reportsGridView.CurrentQuery.DateTo;

            var groupById = reportsGridView.CurrentQuery.GroupById;
            var showColumnIds = reportsGridView.CurrentQuery.ShowColumnIds;

            var dateFormatId = reportsGridView.DateFormatId;

            var reportTotalView = new ReportTotalView(groupById, showColumnIds, dateFormatId, dateFrom, dateTo);

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

                        return reportTotalView.GetView(timeEntriesGroupByProjects);
                    }

                    case (int) ReportsGroupByIds.User:
                    {
                        var timeEntriesGroupByMembers = filteredTimeEntries
                            .GroupBy(i => i.Member)
                            .OrderBy(x => x.Key.FullName)
                            .ToDictionary(key => key.Key, value => value.OrderBy(x => x.Date).ToList());

                        return reportTotalView.GetView(timeEntriesGroupByMembers);
                    }

                    case (int) ReportsGroupByIds.Date:
                    {
                        var timeEntriesGroupByDate = filteredTimeEntries
                            .GroupBy(i => i.Date)
                            .OrderBy(x => x.Key)
                            .ToDictionary(key => key.Key, key => key.OrderBy(x => x.Date).ToList());

                        return reportTotalView.GetView(timeEntriesGroupByDate);
                    }

                    case (int) ReportsGroupByIds.Client:
                    {
                        var timeEntriesGroupByClients = filteredTimeEntries
                            .GroupBy(i => i.Project.Client == null ? CreateWithOutClientInstance() : i.Project.Client)
                            .OrderBy(x => x.Key.Name)
                            .ToDictionary(key => key.Key, value => value.OrderBy(x => x.Date).ToList());

                        return reportTotalView.GetView(timeEntriesGroupByClients);
                    }
                }
            }

            return reportTotalView;
        }

        public void CheckAndSaveCurrentQuery(ReportsGridView reportsGridView)
        {
            var queryDateFrom = reportsGridView.CurrentQuery.DateFrom;
            var queryDateTo = reportsGridView.CurrentQuery.DateTo;

            var memberImpersonatedId = MemberImpersonated.Id;
            var currentQuery = reportsGridView.CurrentQuery;

            var dateStaticId = reportsGridView.CurrentQuery.DateStaticId;
            if (dateStaticId != null)
            {
                var dateStaticExtend = CreateDateStaticExtend(dateStaticId);
                var calculateByStaticIdDateFrom = dateStaticExtend.DateFrom;
                var calculateByStaticIdDateTo = dateStaticExtend.DateTo;

                if (queryDateFrom != calculateByStaticIdDateFrom && queryDateTo != calculateByStaticIdDateTo)
                {
                    throw new CoralTimeDangerException("DateFrom, DateTo from query not equals new DateFrom, DateTo that calculated by DateStaticId");
                }
            }

            _reportsSettingsService.UpdateCurrentQuery(currentQuery, memberImpersonatedId);
        }

        #endregion

        #region Get DropDowns and Grid. Filtration By / Grouping By: Projects, Users, Dates, Clients. (Common methods)

        private List<TimeEntry> GetFilteredTimeEntries(ReportsGridView reportsGridView)
        {
            var memberImpersonated = MemberImpersonated;

            var dateFrom = new DateTime();
            var dateTo = new DateTime();

            FillDatesByDateStaticOrDateFromTo(reportsGridView, ref dateFrom, ref dateTo);

            // By Dates (default grouping, i.e. "Group by None"; direct order).
            var timeEntriesByDateOfUser = GetTimeEntryByDate(memberImpersonated, dateFrom, dateTo);

            // By Projects.
            if (reportsGridView.CurrentQuery?.ProjectIds != null && reportsGridView.CurrentQuery.ProjectIds.Length > 0)
            {
                CheckAndSetIfInFilterChooseSingleProject(reportsGridView, timeEntriesByDateOfUser);

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
                var dateStaticExtend = CreateDateStaticExtend(dateStaticId);

                dateFrom = dateStaticExtend.DateFrom;
                dateTo = dateStaticExtend.DateTo;
            }

            if (isFilledOnlyDateFromDateTo)
            {
                dateFrom = (DateTime) reportsGridView.CurrentQuery?.DateFrom;
                dateTo = (DateTime) reportsGridView.CurrentQuery?.DateTo;
            }
        }

        private void CheckAndSetIfInFilterChooseSingleProject(ReportsGridView reportsGridData, IQueryable<TimeEntry> timeEntriesByDateOfUser)
        {
            if (reportsGridData.CurrentQuery.ProjectIds.Length == 1)
            {
                var singleFilteredProjectId = reportsGridData.CurrentQuery.ProjectIds.FirstOrDefault();
                SingleFilteredProjectName = Uow.ProjectRepository.LinkedCacheGetById(singleFilteredProjectId).Name;
            }
        }

        private IQueryable<TimeEntry> GetTimeEntryByDate(Member currentMember, DateTime dateFrom, DateTime dateTo)
        {
            // #0 Get timeEntriesByDate.s
            var timeEntriesByDate = Uow.TimeEntryRepository.GetQueryWithIncludes()
                .Include(x => x.Project).ThenInclude(x => x.Client)
                .Include(x => x.Member.User)
                .Include(x => x.TaskType)
                .Where(t => t.Date.Date >= dateFrom.Date && t.Date.Date <= dateTo.Date && t.TimeTimerStart <= 0); // TODO update logic to set values for start/stop timer in TimeEntry create method!

            #region Constrain for Admin: return all TimeEntries.

            if (currentMember.User.IsAdmin)
            {
                return timeEntriesByDate;
            }

            #endregion

            #region Constrain for Member. return only TimeEntries that manager is assign.

            if (!currentMember.User.IsAdmin && !currentMember.User.IsManager)
            {
                // #1. TimeEntries. Get tEntries for this member.
                timeEntriesByDate = timeEntriesByDate.Where(t => t.MemberId == currentMember.Id);
            }

            #endregion

            #region Constrain for Manager : return #1 TimeEntries that currentMember is assign, #2 TimeEntries for not assign users at Projects (but TEntries was saved), #4 TimeEntries with global projects that not contains in result.

            if (!currentMember.User.IsAdmin && currentMember.User.IsManager)
            {
                var managerRoleId = Uow.ProjectRoleRepository.LinkedCacheGetList().FirstOrDefault(r => r.Name == ProjectRoleManager).Id;

                var managerProjectIds = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                    .Where(r => r.MemberId == currentMember.Id && r.RoleId == managerRoleId)
                    .Select(x => x.ProjectId)
                    .ToArray();

                // #1. TimeEntries. Get tEntries for this member and tEntries that is current member is Manager!.
                timeEntriesByDate = timeEntriesByDate.Where(t => t.MemberId == currentMember.Id || managerProjectIds.Contains(t.ProjectId));
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