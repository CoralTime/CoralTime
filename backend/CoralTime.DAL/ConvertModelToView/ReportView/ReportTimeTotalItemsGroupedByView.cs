using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        private static ReportItemsView MapTimeEntryToReportGridItemsView(TimeEntry timeEntry, int groupById, int[] showColumnIds)
        {
            return new ReportItemsView
            {
                ClientId = timeEntry.Project.ClientId,
                ProjectId = timeEntry.Project.Id,
                MemberId = timeEntry.Member.Id,
                TaskId = timeEntry.TaskType.Id,

                ClientName = groupById == (int) ReportsGroupBy.Client ? null : timeEntry.Project.Name,
                ProjectName = groupById == (int) ReportsGroupBy.Project ? null : timeEntry.Project.Name,
                MemberName = groupById == (int) ReportsGroupBy.Member ? null : timeEntry.Member.FullName,   

                Date = groupById == (int) ReportsGroupBy.Date
                    ? null 
                    : showColumnIds.Contains((int) ShowColumnModelIds.ShowDate) ? (DateTime?)timeEntry.Date : null, 

                TaskName = timeEntry.TaskType.Name,

                Notes = showColumnIds.Contains((int) ShowColumnModelIds.ShowNotes) ? timeEntry.Description : null, 

                TimeValues = new TimeValuesView
                {
                    TimeFrom = showColumnIds.Contains((int) ShowColumnModelIds.ShowStartFinish) ? (int?) timeEntry.TimeFrom : null, 
                    TimeTo = showColumnIds.Contains((int) ShowColumnModelIds.ShowStartFinish) ? (int?) timeEntry.TimeTo : null,
                    TimeActual = timeEntry.TimeActual,
                    TimeEstimated = showColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime) ? (int?) timeEntry.TimeEstimated : null 
                }
            };
        }

        public static ReportTotalView GetView<TGroupBy>(this ReportTotalView reportTimeTotalItemsGroupedByView, Dictionary<TGroupBy, List<TimeEntry>> dictionaryOfGroupsTimeEntries, int groupById, int[] showColumnIds)
        {
            foreach (var dictionaryOfGroupTimeEntries in dictionaryOfGroupsTimeEntries)
            {
                var reportItemsGroupByTypeView = new ReportTotalForGroupTypeView(groupById).GetViewReportItemsGroupByTypeView(dictionaryOfGroupTimeEntries, groupById, showColumnIds);

                //reportItemsGroupByTypeView.GroupByType.GroupByTypeId == ;
                //reportItemsGroupByTypeView.GroupByType.GroupByTypeDisplayName = reportsGridView.CurrentQuery.GroupById;

                reportTimeTotalItemsGroupedByView.TimeTotal.TimeActualTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeActualTotalFor;
                reportTimeTotalItemsGroupedByView.TimeTotal.TimeEstimatedTotal += reportItemsGroupByTypeView.TimeTotalFor.TimeEstimatedTotalFor;
                reportTimeTotalItemsGroupedByView.GroupedItems.Add(reportItemsGroupByTypeView);
            }

            reportTimeTotalItemsGroupedByView.TimeTotal.TimeEstimatedTotal = showColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTimeTotalItemsGroupedByView.TimeTotal.TimeEstimatedTotal
                : null;

            return reportTimeTotalItemsGroupedByView;
        }

        private static ReportTotalForGroupTypeView GetViewReportItemsGroupByTypeView<TGroupBy>(this ReportTotalForGroupTypeView reportItemsGroupByTypeView, KeyValuePair<TGroupBy, List<TimeEntry>> dictionaryOfGroupTimeEntries, int groupById, int[] showColumnIds)
        {
            var reportTimeTotalForItemsView = new ReportTotalForView().GetViewTimeTotalForItems(dictionaryOfGroupTimeEntries.Value, groupById, showColumnIds);

            switch (dictionaryOfGroupTimeEntries.Key)
            {
                case Project project:
                {
                    reportItemsGroupByTypeView.GroupByType.ProjectId = project.Id;
                    reportItemsGroupByTypeView.GroupByType.ProjectName = project.Name;

                    break;
                }

                case Member member:
                {
                    reportItemsGroupByTypeView.GroupByType.MemberId = member.Id;
                    reportItemsGroupByTypeView.GroupByType.MemberName = member.FullName;

                    break;
                }

                case DateTime dateTime:
                {
                    reportItemsGroupByTypeView.GroupByType.Date = dateTime;

                    break;
                }

                case Client client:
                {
                    reportItemsGroupByTypeView.GroupByType.ClientId = client.Id;
                    reportItemsGroupByTypeView.GroupByType.ClientName = client.Name;

                    break;
                }
            }

            reportItemsGroupByTypeView.TimeTotalFor = reportTimeTotalForItemsView.TimeTotalFor;
            reportItemsGroupByTypeView.Items = reportTimeTotalForItemsView.Items;

            return reportItemsGroupByTypeView;
        }

        private static ReportTotalForView GetViewTimeTotalForItems(this ReportTotalForView reportTimeTotalForItemsView, List<TimeEntry> groupedTimeEntries, int groupById, int[] showColumnIds) 
        {
            reportTimeTotalForItemsView.Items = groupedTimeEntries.Select(x => MapTimeEntryToReportGridItemsView(x, groupById, showColumnIds)).ToList();

            foreach (var groupingTimeEntries in reportTimeTotalForItemsView.Items)
            {
                reportTimeTotalForItemsView.TimeTotalFor.TimeActualTotalFor += groupingTimeEntries.TimeValues.TimeActual;
                reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor += groupingTimeEntries.TimeValues.TimeEstimated;
            }

            reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor = showColumnIds.Contains((int) ShowColumnModelIds.ShowEstimatedTime)
                ? reportTimeTotalForItemsView.TimeTotalFor.TimeEstimatedTotalFor
                : null;

            return reportTimeTotalForItemsView;
        }
    }
}