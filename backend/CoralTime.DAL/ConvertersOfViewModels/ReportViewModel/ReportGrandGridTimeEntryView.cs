using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        public static ReportsGrandGridTimeEntryView GetViewReportsGrandGridTimeEntries(this ReportsGrandGridTimeEntryView reportsGridEntitiesTimeEntry, Dictionary<int, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridClientView = GetReportGridTimeEntryViewList(timeEntries, _mapper);

            foreach (var groupingTimeEntries in reportGridClientView)
            {
                reportsGridEntitiesTimeEntry.GrandActualTime += groupingTimeEntries.TotalActualTime;
                reportsGridEntitiesTimeEntry.GrandEstimatedTime += groupingTimeEntries.TotalEstimatedTime;
            }

            reportsGridEntitiesTimeEntry.ReportsGridView = reportGridClientView;

            return reportsGridEntitiesTimeEntry;
        }

        private static List<ReportGridTimeEntryView> GetReportGridTimeEntryViewList(Dictionary<int, IEnumerable<TimeEntry>> timeEntriesGroupByNone, IMapper _mapper)
        {
            var timeEntriesGroupByNoneView = timeEntriesGroupByNone.Select(x => x.GetViewReportGridNone(_mapper)).ToList();

            var reportGridTimeEntryView = new ReportGridTimeEntryView
            {
                // Get inner separated items from each separated entity and collapce it to single list with many items.
                Items = timeEntriesGroupByNoneView.SelectMany(item => item.Items),
            };

            // Get value from inner item of each separated entity, then calculate sum operation.
            foreach (var tEntry in timeEntriesGroupByNoneView)
            {
                reportGridTimeEntryView.TotalActualTime += tEntry.TotalActualTime;
                reportGridTimeEntryView.TotalEstimatedTime += tEntry.TotalEstimatedTime;
            }

            // Create list with only single entity.
            return new List<ReportGridTimeEntryView> { reportGridTimeEntryView };
        }

        private static ReportGridTimeEntryView GetViewReportGridNone(this KeyValuePair<int, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridDateView = new ReportGridTimeEntryView();

            SetReportsGridItemViewValues(timeEntries, reportGridDateView, _mapper);

            return reportGridDateView;
        }
    }
}
