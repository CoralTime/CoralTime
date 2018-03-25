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

            reportItemsView.ClientName = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Client 
                ? null 
                : timeEntry.Project.Client == null ? WithoutClient.Name : timeEntry.Project.Client.Name;
            reportItemsView.ProjectName = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Project ? null : timeEntry.Project.Name;
            reportItemsView.MemberName = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Member ? null : timeEntry.Member.FullName;
            reportItemsView.Date = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Date
                ? null
                : reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowDate) 
                    ? (DateTime?) timeEntry.Date 
                    : null;
       
            reportItemsView.Notes = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowNotes) ? timeEntry.Description ?? string.Empty : null;
            reportItemsView.TimeValues = new TimeValuesView
            {
                TimeFrom = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowStartFinish) ? (int?) timeEntry.TimeFrom : null,
                TimeTo = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowStartFinish) ? (int?) timeEntry.TimeTo : null,
                TimeActual = timeEntry.TimeActual,
                TimeEstimated = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime) ? (int?) timeEntry.TimeEstimated : null 
            };

            reportItemsView.DisplayNameClient = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Client ? null : reportItemsView.ClientId == WithoutClient.Id ? null : reportItemsView.DisplayNameClient;
            reportItemsView.DisplayNameProject = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Project ? null : reportItemsView.DisplayNameProject;
            reportItemsView.DisplayNameMember = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Member ? null : reportItemsView.DisplayNameMember;
            reportItemsView.DisplayNameDate = reportItemsView.GroupByTypeId == (int) ReportsGroupBy.Date
                ? null
                : reportItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowDate) ? reportItemsView.DisplayNameDate : null;
            reportItemsView.DisplayNameNotes = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowNotes) ? reportItemsView.DisplayNameNotes : null;
            reportItemsView.DisplayNameTimeFrom = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowStartFinish) ? reportItemsView.DisplayNameTimeFrom : null;
            reportItemsView.DisplayNameTimeTo = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowStartFinish) ? reportItemsView.DisplayNameTimeTo : null;
            reportItemsView.DisplayNameTimeEstimated = reportItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime) ? reportItemsView.DisplayNameTimeEstimated : null;

            return reportItemsView;
        }

        public static ReportTotalView GetView<TGroupBy>(this ReportTotalView reportTimeTotalItemsGroupedByView, Dictionary<TGroupBy, List<TimeEntry>> dictionaryOfGroupsTimeEntries)
        {
            var groupByTypeId = reportTimeTotalItemsGroupedByView.GroupByTypeId;
            var showColumnIds = reportTimeTotalItemsGroupedByView.ShowColumnIds;

            var dateFormatId = reportTimeTotalItemsGroupedByView.DateFormatId;
            var dateFrom = reportTimeTotalItemsGroupedByView.PeriodCell.DateFrom;
            var dateTo = reportTimeTotalItemsGroupedByView.PeriodCell.DateTo;

            reportTimeTotalItemsGroupedByView.PeriodCell.DisplayNamePeriodValue = GetPeriodCellvalue(dateFrom, dateTo, dateFormatId);

            reportTimeTotalItemsGroupedByView.GroupedItems = new List<ReportTotalForGroupTypeView>();

            foreach (var dictionaryOfGroupTimeEntries in dictionaryOfGroupsTimeEntries)
            {
                var reportItemsGroupByTypeView = new ReportTotalForGroupTypeView(groupByTypeId, showColumnIds, dateFormatId).GetView(dictionaryOfGroupTimeEntries);

                reportTimeTotalItemsGroupedByView.TimeTotal.TimeActualTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeActualTotalFor;
                reportTimeTotalItemsGroupedByView.TimeTotal.TimeEstimatedTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeEstimatedTotalFor;
                reportTimeTotalItemsGroupedByView.GroupedItems.Add(reportItemsGroupByTypeView);
            }

            reportTimeTotalItemsGroupedByView.TimeTotal.TimeEstimatedTotal = showColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTimeTotalItemsGroupedByView.TimeTotal.TimeEstimatedTotal
                : null;

            return reportTimeTotalItemsGroupedByView;
        }

        private static ReportTotalForGroupTypeView GetView<TGroupBy>(this ReportTotalForGroupTypeView reportItemsGroupByTypeView, KeyValuePair<TGroupBy, List<TimeEntry>> dictionaryOfGroupTimeEntries)
        {
            var groupByTypeId = reportItemsGroupByTypeView.GroupByTypeId;
            var showColumnIds = reportItemsGroupByTypeView.ShowColumnIds;

            var reportTimeTotalForItemsView = new ReportTotalForView(groupByTypeId, showColumnIds).GetView(dictionaryOfGroupTimeEntries.Value);

            switch (dictionaryOfGroupTimeEntries.Key)
            {
                case Project project:
                {
                    reportItemsGroupByTypeView.GroupByType.ProjectId = project.Id;
                    reportItemsGroupByTypeView.GroupByType.ProjectName = project.Name;

                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = reportItemsGroupByTypeView.DisplayNameProject + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = project.Name;
                    reportItemsGroupByTypeView.DisplayNameProject = reportItemsGroupByTypeView.GroupByTypeId == (int)ReportsGroupBy.Project ? null : reportItemsGroupByTypeView.DisplayNameProject;

                    break;
                }

                case Member member:
                {
                    reportItemsGroupByTypeView.GroupByType.MemberId = member.Id;
                    reportItemsGroupByTypeView.GroupByType.MemberName = member.FullName;

                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = reportItemsGroupByTypeView.DisplayNameMember + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = member.FullName;
                    reportItemsGroupByTypeView.DisplayNameMember = reportItemsGroupByTypeView.GroupByTypeId == (int)ReportsGroupBy.Member ? null : reportItemsGroupByTypeView.DisplayNameMember;

                    break;
                }

                case DateTime dateTime:
                {
                    reportItemsGroupByTypeView.GroupByType.Date = dateTime;
                    
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = reportItemsGroupByTypeView.DisplayNameDate + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = UpdateDateFormat(dateTime, reportItemsGroupByTypeView.DateFormatId);
                    reportItemsGroupByTypeView.DisplayNameDate = reportItemsGroupByTypeView.GroupByTypeId == (int)ReportsGroupBy.Date ? null : reportItemsGroupByTypeView.DisplayNameDate;

                    break;
                }

                case Client client:
                {
                    reportItemsGroupByTypeView.GroupByType.ClientId = client.Id;
                    reportItemsGroupByTypeView.GroupByType.ClientName = client.Name;

                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = client.Id == WithoutClient.Id ? null : reportItemsGroupByTypeView.DisplayNameClient + ": ";
                    reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayNameValue = client.Name;
                    reportItemsGroupByTypeView.DisplayNameClient = reportItemsGroupByTypeView.GroupByTypeId == (int)ReportsGroupBy.Client ? null : reportItemsGroupByTypeView.DisplayNameClient;

                    break;
                }
            }

            reportItemsGroupByTypeView.DisplayNameDate = reportItemsGroupByTypeView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowDate) ? reportItemsGroupByTypeView.DisplayNameDate : null;
            reportItemsGroupByTypeView.DisplayNameNotes = reportItemsGroupByTypeView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowNotes) ? reportItemsGroupByTypeView.DisplayNameNotes : null;
            reportItemsGroupByTypeView.DisplayNameTimeFrom = reportItemsGroupByTypeView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? reportItemsGroupByTypeView.DisplayNameTimeFrom : null;
            reportItemsGroupByTypeView.DisplayNameTimeTo = reportItemsGroupByTypeView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? reportItemsGroupByTypeView.DisplayNameTimeTo : null;
            reportItemsGroupByTypeView.DisplayNameTimeEstimated = reportItemsGroupByTypeView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowEstimatedTime) ? reportItemsGroupByTypeView.DisplayNameTimeEstimated : null;

            reportItemsGroupByTypeView.TimeTotalFor = reportTimeTotalForItemsView.TimeTotalFor;
            reportItemsGroupByTypeView.Items = reportTimeTotalForItemsView.Items;

            return reportItemsGroupByTypeView;
        }

        private static ReportTotalForView GetView(this ReportTotalForView reportTimeTotalForItemsView, List<TimeEntry> groupedTimeEntries)
        {
            var groupByTypeId = reportTimeTotalForItemsView.GroupByTypeId;
            var showColumnIds = reportTimeTotalForItemsView.ShowColumnIds;

            reportTimeTotalForItemsView.Items = groupedTimeEntries.Select(x => MapTimeEntryToReportGridItemsView(x, new ReportItemsView(groupByTypeId, showColumnIds))).ToList();

            foreach (var groupingTimeEntries in reportTimeTotalForItemsView.Items)
            {
                reportTimeTotalForItemsView.TimeTotalFor.TimeActualTotalFor += groupingTimeEntries.TimeValues.TimeActual;
                reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor += groupingTimeEntries.TimeValues.TimeEstimated;
            }

            reportTimeTotalForItemsView.TimeTotalFor.DisplayNameTimeEstimatedTotalFor = reportTimeTotalForItemsView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowEstimatedTime)
                ? reportTimeTotalForItemsView.TimeTotalFor.DisplayNameTimeEstimatedTotalFor
                : null;
            reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor = reportTimeTotalForItemsView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor
                : null;

            return reportTimeTotalForItemsView;
        }
    }
}