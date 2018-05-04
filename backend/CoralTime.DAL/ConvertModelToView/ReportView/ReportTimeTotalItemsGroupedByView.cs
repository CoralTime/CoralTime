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
            if (string.IsNullOrEmpty(value))
            {

            }
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

            reportItemsView.ClientName = reportItemsView.GroupByTypeId == (int)ReportsGroupByIds.Client
                ? null
                : timeEntry.Project.Client == null ? WithoutClient.Name : timeEntry.Project.Client.Name;
            reportItemsView.ProjectName = reportItemsView.GroupByTypeId == (int)ReportsGroupByIds.Project ? null : timeEntry.Project.Name;
            reportItemsView.MemberName = reportItemsView.GroupByTypeId == (int)ReportsGroupByIds.User ? null : timeEntry.Member.FullName;
            reportItemsView.Date = reportItemsView.GroupByTypeId == (int)ReportsGroupByIds.Date
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

            reportTotalView.PeriodCell.DisplayNamePeriodValue = GetPeriodCellvalue(dateFrom, dateTo, dateFormatId);

            HideDisplayNamesByGroupingAndShowColumnsIds(reportTotalView);

            reportTotalView.GroupedItems = new List<ReportTotalForGroupTypeView>();

            foreach (var dictionaryOfGroupTimeEntries in dictionaryOfGroupsTimeEntries)
            {
                var reportItemsGroupByTypeView = new ReportTotalForGroupTypeView(groupByTypeId, showColumnIds, dateFormatId).GetView(dictionaryOfGroupTimeEntries);

                reportTotalView.TimeTotal.TimeActualTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeActualTotalFor;
                reportTotalView.TimeTotal.TimeEstimatedTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeEstimatedTotalFor;
                reportTotalView.GroupedItems.Add(reportItemsGroupByTypeView);

                reportItemsGroupByTypeView.DisplayNames = reportTotalView.DisplayNames;
            }

            reportTotalView.TimeTotal.TimeEstimatedTotal = showColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTotalView.TimeTotal.TimeEstimatedTotal
                : null;

            return reportTotalView;
        }

        private static ReportTotalForGroupTypeView GetView<TGroupBy>(this ReportTotalForGroupTypeView reportTotalForGroupTypeView, KeyValuePair<TGroupBy, List<TimeEntry>> dictionaryOfGroupTimeEntries)
        {
            var groupByTypeId = reportTotalForGroupTypeView.GroupByTypeId;
            var showColumnIds = reportTotalForGroupTypeView.ShowColumnIds;

            var reportTimeTotalForItemsView = new ReportTotalForView(groupByTypeId, showColumnIds).GetView(dictionaryOfGroupTimeEntries.Value);

            switch (dictionaryOfGroupTimeEntries.Key)
            {
                case Project project:
                {
                    reportTotalForGroupTypeView.GroupByType.ProjectId = project.Id;
                    reportTotalForGroupTypeView.GroupByType.ProjectName = project.Name;

                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayName = reportTotalForGroupTypeView.DisplayNames.DisplayNameProject + ": ";
                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayNameValue = project.Name;
                    //reportTimeTotalForItemsView.DisplayNames.DisplayNameProject = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Project ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameProject;

                    break;
                }

                case Member member:
                {
                    reportTotalForGroupTypeView.GroupByType.MemberId = member.Id;
                    reportTotalForGroupTypeView.GroupByType.MemberName = member.FullName;

                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayName = reportTotalForGroupTypeView.DisplayNames.DisplayNameMember + ": ";
                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayNameValue = member.FullName;
                    //reportTimeTotalForItemsView.DisplayNames.DisplayNameMember = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Member ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameMember;

                    break;
                }

                case DateTime dateTime:
                {
                    reportTotalForGroupTypeView.GroupByType.Date = dateTime;
                    
                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayName = reportTotalForGroupTypeView.DisplayNames.DisplayNameDate + ": ";
                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayNameValue = UpdateDateFormat(dateTime, reportTotalForGroupTypeView.DateFormatId);
                    //reportTimeTotalForItemsView.DisplayNames.DisplayNameDate = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Date ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameDate;

                    break;
                }

                case Client client:
                {
                    reportTotalForGroupTypeView.GroupByType.ClientId = client.Id;
                    reportTotalForGroupTypeView.GroupByType.ClientName = client.Name;

                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayName = client.Id == WithoutClient.Id ? null : reportTotalForGroupTypeView.DisplayNames.DisplayNameClient + ": ";
                    reportTotalForGroupTypeView.GroupByType.GroupByTypeDisplayNameValue = client.Name;
                    //reportTimeTotalForItemsView.DisplayNames.DisplayNameClient = reportTimeTotalForItemsView.GroupByTypeId == (int)ReportsGroupBy.Client ? null : reportTimeTotalForItemsView.DisplayNames.DisplayNameClient;

                    break;
                }
            }

            //reportTotalForGroupTypeView.DisplayNames = reportTimeTotalForItemsView.DisplayNames;
            reportTotalForGroupTypeView.TimeTotalFor = reportTimeTotalForItemsView.TimeTotalFor;
            reportTotalForGroupTypeView.Items = reportTimeTotalForItemsView.Items;

            return reportTotalForGroupTypeView;
        }

        private static ReportTotalForView GetView(this ReportTotalForView reportTotalForView, List<TimeEntry> groupedTimeEntries)
        {
            var groupByTypeId = reportTotalForView.GroupByTypeId;
            var showColumnIds = reportTotalForView.ShowColumnIds;

            reportTotalForView.Items = groupedTimeEntries.Select(x => MapTimeEntryToReportGridItemsView(x, new ReportItemsView(groupByTypeId, showColumnIds))).ToList();

            foreach (var groupingTimeEntries in reportTotalForView.Items)
            {
                reportTotalForView.TimeTotalFor.TimeActualTotalFor += groupingTimeEntries.TimeValues.TimeActual;
                reportTotalForView.TimeTotalFor.TimeEstimatedTotalFor += groupingTimeEntries.TimeValues.TimeEstimated;
            }

            reportTotalForView.TimeTotalFor.TimeEstimatedTotalFor = reportTotalForView.ShowColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTotalForView.TimeTotalFor.TimeEstimatedTotalFor
                : null;

            return reportTotalForView;
        }

        private static void HideDisplayNamesByGroupingAndShowColumnsIds(ReportTotalView reportTotalView)
        {
            reportTotalView.DisplayNames.DisplayNameProject = reportTotalView.GroupByTypeId == (int)ReportsGroupByIds.Project ? null : reportTotalView.DisplayNames.DisplayNameProject;
            reportTotalView.DisplayNames.DisplayNameMember = reportTotalView.GroupByTypeId == (int)ReportsGroupByIds.User ? null : reportTotalView.DisplayNames.DisplayNameMember;
            reportTotalView.DisplayNames.DisplayNameDate = reportTotalView.GroupByTypeId == (int)ReportsGroupByIds.Date ? null : reportTotalView.DisplayNames.DisplayNameDate;
            reportTotalView.DisplayNames.DisplayNameClient = reportTotalView.GroupByTypeId == (int)ReportsGroupByIds.Client ? null : reportTotalView.DisplayNames.DisplayNameClient;

            reportTotalView.DisplayNames.DisplayNameDate = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowDate) ? reportTotalView.DisplayNames.DisplayNameDate : null;
            reportTotalView.DisplayNames.DisplayNameNotes = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowNotes) ? reportTotalView.DisplayNames.DisplayNameNotes : null;
            reportTotalView.DisplayNames.DisplayNameTimeFrom = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? reportTotalView.DisplayNames.DisplayNameTimeFrom : null;
            reportTotalView.DisplayNames.DisplayNameTimeTo = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowStartFinish) ? reportTotalView.DisplayNames.DisplayNameTimeTo : null;
            reportTotalView.DisplayNames.DisplayNameTimeEstimated = reportTotalView.ShowColumnIds.Contains((int)ShowColumnModelIds.ShowEstimatedTime) ? reportTotalView.DisplayNames.DisplayNameTimeEstimated : null;
        }
    }
}