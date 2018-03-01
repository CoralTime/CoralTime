using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        private static void SetReportsExportTotalForAndItemsValues<T>(KeyValuePair<T, IEnumerable<TimeEntry>> timeEntries, IReportsTotalForGridItemsView exportTotalForItemsView, IMapper _mapper)
        {
            CalculateTotalForValues(out var totalForActualTime, out var totalForEstimatedTime, timeEntries.Value);

            exportTotalForItemsView.TotalForActualTime = totalForActualTime;
            exportTotalForItemsView.TotalForEstimatedTime = totalForEstimatedTime;
            exportTotalForItemsView.Items = timeEntries.Value.Select(x => x.GetViewReportsGridItem(_mapper));
        }

        private static void CalculateTotalForValues(out int totalForActualTime, out int totalForEstimatedTime, IEnumerable<TimeEntry> timeEntries)
        {
            totalForActualTime = 0;
            totalForEstimatedTime = 0;

            foreach (var timeEntry in timeEntries)
            {
                totalForActualTime += timeEntry.Time;
                totalForEstimatedTime += timeEntry.PlannedTime;
            }
        }
    }
}