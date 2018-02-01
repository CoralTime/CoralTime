using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        private static void SetReportsGridItemViewValues<T>(KeyValuePair<T, IEnumerable<TimeEntry>> timeEntries, IReportsGridTotalItemsView reportGridView, IMapper _mapper)
        {
            CalculateTotalEstimatedActualTime(out var totalActualTime, out var totalEstimatedTime, timeEntries.Value);

            reportGridView.TotalActualTime = totalActualTime;
            reportGridView.TotalEstimatedTime = totalEstimatedTime;
            reportGridView.Items = timeEntries.Value.Select(x => x.GetViewReportsGridItem(_mapper));
        }

        private static void CalculateTotalEstimatedActualTime(out int totalActualTime, out int totalEstimatedTime, IEnumerable<TimeEntry> timeEntries)
        {
            totalActualTime = 0;
            totalEstimatedTime = 0;

            foreach (var item in timeEntries)
            {
                totalActualTime += item.Time;
                totalEstimatedTime += item.PlannedTime;
            }
        }
    }
}