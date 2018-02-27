using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        public static ReportsTotalGridByDatesView GetViewReportsTotalGridDatess(this ReportsTotalGridByDatesView reportsGridEntitiesClients, Dictionary<DateTime, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridDateView = timeEntries.Select(x => x.GetViewReportGridDate(_mapper));

            foreach (var groupingTimeEntries in reportGridDateView)
            {
                reportsGridEntitiesClients.TotalActualTime += groupingTimeEntries.TotalForActualTime;
                reportsGridEntitiesClients.TotalEstimatedTime += groupingTimeEntries.TotalForEstimatedTime;
            }

            reportsGridEntitiesClients.ReportsGridView = reportGridDateView;

            return reportsGridEntitiesClients;
        }

        private static ReportTotalForGridDateView GetViewReportGridDate(this KeyValuePair<DateTime, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridDateView = new ReportTotalForGridDateView();

            SetReportsExportTotalForAndItemsValues(timeEntries, reportGridDateView, _mapper);

            reportGridDateView.Date = timeEntries.Key.Date;

            return reportGridDateView;
        }
    }
}