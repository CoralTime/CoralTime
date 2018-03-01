using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ReportsTotalGridTimeEntryView GetViewReportsTotalGridTimeEntries(this ReportsTotalGridTimeEntryView reportsGridEntitiesTimeEntry, Dictionary<int, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridClientView = GetReportGridTimeEntryViewList(timeEntries, _mapper);

            foreach (var groupingTimeEntries in reportGridClientView)
            {
                reportsGridEntitiesTimeEntry.TotalActualTime += groupingTimeEntries.TotalForActualTime;
                reportsGridEntitiesTimeEntry.TotalEstimatedTime += groupingTimeEntries.TotalForEstimatedTime;
            }

            reportsGridEntitiesTimeEntry.ReportsGridView = reportGridClientView;

            return reportsGridEntitiesTimeEntry;
        }

        private static List<ReportTotalForGridTimeEntryView> GetReportGridTimeEntryViewList(Dictionary<int, IEnumerable<TimeEntry>> timeEntriesGroupByNone, IMapper _mapper)
        {
            var timeEntriesGroupByNoneView = timeEntriesGroupByNone.Select(x => x.GetViewReportGridNone(_mapper)).ToList();

            var reportGridTimeEntryView = new ReportTotalForGridTimeEntryView
            {
                // Get inner separated items from each separated entity and collapce it to single list with many items.
                Items = timeEntriesGroupByNoneView.SelectMany(item => item.Items),
            };

            // Get value from inner item of each separated entity, then calculate sum operation.
            foreach (var tEntry in timeEntriesGroupByNoneView)
            {
                reportGridTimeEntryView.TotalForActualTime += tEntry.TotalForActualTime;
                reportGridTimeEntryView.TotalForEstimatedTime += tEntry.TotalForEstimatedTime;
            }

            // Create list with only single entity.
            return new List<ReportTotalForGridTimeEntryView> { reportGridTimeEntryView };
        }

        private static ReportTotalForGridTimeEntryView GetViewReportGridNone(this KeyValuePair<int, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridDateView = new ReportTotalForGridTimeEntryView();

            SetReportsExportTotalForAndItemsValues(timeEntries, reportGridDateView, _mapper);

            return reportGridDateView;
        }
    }
}
