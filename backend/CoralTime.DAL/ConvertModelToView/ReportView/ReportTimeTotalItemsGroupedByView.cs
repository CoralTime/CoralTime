using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static string UpdateDateFormat(DateTime? dateTime, int? dateFormatId)
        {
            if (dateTime != null)
            {
                var dateFormat = new GetDateFormat().GetDateFormaDotNetById(dateFormatId ?? 0);
                var valueSingleFromProp = ((DateTime)dateTime).ToString(dateFormat, CultureInfo.InvariantCulture);

                return valueSingleFromProp;
            }

            return null;
        }

        public static string UpdateTimeFormatForValue(string value)
        {
            var time = TimeSpan.FromSeconds(Int32.Parse(value));
            if (time.TotalHours == 0)
            {
                value = string.Empty;
            }
            else
            {
                value = time.TotalHours <= 99.99 
                    ? $"{(int)time.TotalHours:D2}:{time.Minutes:D2}" 
                    : $"{(int)time.TotalHours}:{time.Minutes:D2}";
            }

            return value;
        }

        private  static string GetPeriodCellvalue(DateTime? dateFrom, DateTime? dateTo, int? dateFormatId)
        {
            var value = UpdateDateFormat(dateFrom, dateFormatId) + " - " + UpdateDateFormat(dateTo, dateFormatId);
            return value;
        }

        private static ReportItemsView MapTimeEntryToReportGridItemsView(TimeEntry timeEntry, ReportItemsView reportItemsView)
        {
            reportItemsView.ClientId = timeEntry.Project.Client == null ? WithoutClient.Id : timeEntry.Project.Client.Id;
            reportItemsView.ProjectId = timeEntry.Project.Id;
            reportItemsView.MemberId = timeEntry.Member.Id;
            reportItemsView.TaskId = timeEntry.TaskType.Id;
            reportItemsView.TaskName = timeEntry.TaskType.Name;

            reportItemsView.ClientName = reportItemsView.GroupByTypeId == (int)ReportsGroupBy.Client
                ? null
                : timeEntry.Project.Client == null ? WithoutClient.Name : timeEntry.Project.Client.Name;
            reportItemsView.ProjectName = reportItemsView.GroupByTypeId == (int)ReportsGroupBy.Project ? null : timeEntry.Project.Name;
            reportItemsView.MemberName = reportItemsView.GroupByTypeId == (int)ReportsGroupBy.Member ? null : timeEntry.Member.FullName;
            reportItemsView.Date = reportItemsView.GroupByTypeId == (int)ReportsGroupBy.Date
                ? null
                : reportItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowDate)
                    ? (DateTime?)timeEntry.Date
                    : null;

            reportItemsView.Notes = reportItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowNotes) ? timeEntry.Description ?? string.Empty : null;
            reportItemsView.TimeValues = new TimeValuesView
            {
                TimeFrom = reportItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? (int?)timeEntry.TimeFrom : null,
                TimeTo = reportItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? (int?)timeEntry.TimeTo : null,
                TimeActual = timeEntry.TimeActual,
                TimeEstimated = reportItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowEstimatedTime) ? (int?)timeEntry.TimeEstimated : null
            };

            return reportItemsView;
        }

        public static ReportTotalView GetView<TGroupBy>(this ReportTotalView reportTotalView, Dictionary<TGroupBy, List<TimeEntry>> dictionaryOfGroupsTimeEntries)
        {
            var groupByTypeId = reportTotalView.GroupByTypeId;
            var showColumnIds = reportTotalView.ShowColumnIds;

            var dateFormatId = reportTotalView.DateFormatId;
            var dateFrom = reportTotalView.PeriodCell.DateFrom;
            var dateTo = reportTotalView.PeriodCell.DateTo;
            var displayNames = reportTotalView.DisplayNames;

            reportTotalView.PeriodCell.DisplayNamePeriodValue = GetPeriodCellvalue(dateFrom, dateTo, dateFormatId);

            HideDisplayNamesByGroupingAndShowColumnsIds(reportTotalView);

            reportTotalView.GroupedItems = new List<ReportTotalForGroupTypeView>();

            foreach (var dictionaryOfGroupTimeEntries in dictionaryOfGroupsTimeEntries)
            {
                var reportItemsGroupByTypeView = new ReportTotalForGroupTypeView(groupByTypeId, showColumnIds, dateFormatId, displayNames).GetView(dictionaryOfGroupTimeEntries);

                reportTotalView.TimeTotal.TimeActualTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeActualTotalFor;
                reportTotalView.TimeTotal.TimeEstimatedTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeEstimatedTotalFor;
                reportTotalView.GroupedItems.Add(reportItemsGroupByTypeView);
            }

            reportTotalView.TimeTotal.TimeEstimatedTotal = showColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTotalView.TimeTotal.TimeEstimatedTotal
                : null;

            return reportTotalView;
        }

        private static ReportTotalForGroupTypeView GetView<TGroupBy>(this ReportTotalForGroupTypeView reportItemsGroupByTypeView, KeyValuePair<TGroupBy, List<TimeEntry>> dictionaryOfGroupTimeEntries)
        {
            var groupByTypeId = reportItemsGroupByTypeView.GroupByTypeId;
            var showColumnIds = reportItemsGroupByTypeView.ShowColumnIds;
            var displayNames = reportItemsGroupByTypeView.DisplayNames;

            var reportTimeTotalForItemsView = new ReportTotalForView(groupByTypeId, showColumnIds, displayNames).GetView(dictionaryOfGroupTimeEntries.Value);

            switch (dictionaryOfGroupTimeEntries.Key)
            {
                case Project project:
                {
                    reportItemsGroupByTypeView.GroupByType.ProjectId = project.Id;
                    reportItemsGroupByTypeView.GroupByType.ProjectName = project.Name;

                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = reportItemsGroupByTypeView.DisplayNames.DisplayNameProject + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = project.Name;

                    break;
                }

                case Member member:
                {
                    reportItemsGroupByTypeView.GroupByType.MemberId = member.Id;
                    reportItemsGroupByTypeView.GroupByType.MemberName = member.FullName;

                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = reportItemsGroupByTypeView.DisplayNames.DisplayNameMember + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = member.FullName;

                    break;
                }

                case DateTime dateTime:
                {
                    reportItemsGroupByTypeView.GroupByType.Date = dateTime;
                    
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = reportItemsGroupByTypeView.DisplayNames.DisplayNameDate + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = UpdateDateFormat(dateTime, reportItemsGroupByTypeView.DateFormatId);

                    break;
                }

                case Client client:
                {
                    reportItemsGroupByTypeView.GroupByType.ClientId = client.Id;
                    reportItemsGroupByTypeView.GroupByType.ClientName = client.Name;

                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = client.Id == WithoutClient.Id ? null : reportItemsGroupByTypeView.DisplayNames.DisplayNameClient + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = client.Name;

                    break;
                }
            }

            reportTimeTotalForItemsView.DisplayNames.DisplayNameProject = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Project ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameProject;
            reportTimeTotalForItemsView.DisplayNames.DisplayNameMember = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Member ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameMember;
            reportTimeTotalForItemsView.DisplayNames.DisplayNameDate = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Date ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameDate;
            reportTimeTotalForItemsView.DisplayNames.DisplayNameClient = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Client ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameClient;

            reportItemsGroupByTypeView.TimeTotalFor = reportTimeTotalForItemsView.TimeTotalFor;
            reportItemsGroupByTypeView.Items = reportTimeTotalForItemsView.Items;

            return reportItemsGroupByTypeView;
        }

        private static ReportTotalForView GetView(this ReportTotalForView reportTimeTotalForItemsView, List<TimeEntry> groupedTimeEntries)
        {
            var groupByTypeId = reportTimeTotalForItemsView.GroupByTypeId;
            var showColumnIds = reportTimeTotalForItemsView.ShowColumnIds;
            var displayNames = reportTimeTotalForItemsView.DisplayNames;
            reportTimeTotalForItemsView.Items = groupedTimeEntries.Select(x => MapTimeEntryToReportGridItemsView(x, new ReportItemsView(groupByTypeId, showColumnIds, displayNames))).ToList();

            foreach (var groupingTimeEntries in reportTimeTotalForItemsView.Items)
            {
                reportTimeTotalForItemsView.TimeTotalFor.TimeActualTotalFor += groupingTimeEntries.TimeValues.TimeActual;
                reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor += groupingTimeEntries.TimeValues.TimeEstimated;
            }

            reportTimeTotalForItemsView.TimeTotalFor.DisplayNameTimeEstimatedTotalFor = reportTimeTotalForItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowEstimatedTime) ? reportTimeTotalForItemsView.TimeTotalFor.DisplayNameTimeEstimatedTotalFor : null;
            reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor = reportTimeTotalForItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor
                : null;

            return reportTimeTotalForItemsView;
        }

        private static void HideDisplayNamesByGroupingAndShowColumnsIds(ReportTotalView reportTotalView)
        {
            reportTotalView.DisplayNames.DisplayNameDate = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowDate) ? reportTotalView.DisplayNames.DisplayNameDate : null;
            reportTotalView.DisplayNames.DisplayNameNotes = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowNotes) ? reportTotalView.DisplayNames.DisplayNameNotes : null;
            reportTotalView.DisplayNames.DisplayNameTimeFrom = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? reportTotalView.DisplayNames.DisplayNameTimeFrom : null;
            reportTotalView.DisplayNames.DisplayNameTimeTo = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? reportTotalView.DisplayNames.DisplayNameTimeTo : null;
            reportTotalView.DisplayNames.DisplayNameTimeEstimated = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowEstimatedTime) ? reportTotalView.DisplayNames.DisplayNameTimeEstimated : null;

            reportTotalView.TimeTotal.DisplayNameTimeEstimatedTotal = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowEstimatedTime) ? reportTotalView.TimeTotal.DisplayNameTimeEstimatedTotal : null;
        }
    }
}