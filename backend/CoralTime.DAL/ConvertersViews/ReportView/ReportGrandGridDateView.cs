using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ReportsGrandGridDatesView GetViewReportsGrandGridClients(this ReportsGrandGridDatesView reportsGridEntitiesClients, Dictionary<DateTime, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridDateView = timeEntries.Select(x => x.GetViewReportGridDate(_mapper));

            foreach (var groupingTimeEntries in reportGridDateView)
            {
                reportsGridEntitiesClients.GrandActualTime += groupingTimeEntries.TotalActualTime;
                reportsGridEntitiesClients.GrandEstimatedTime += groupingTimeEntries.TotalEstimatedTime;
            }

            reportsGridEntitiesClients.ReportsGridView = reportGridDateView;

            return reportsGridEntitiesClients;
        }

        private static ReportGridDateView GetViewReportGridDate(this KeyValuePair<DateTime, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridDateView = new ReportGridDateView();

            SetReportsGridItemViewValues(timeEntries, reportGridDateView, _mapper);

            reportGridDateView.Date = timeEntries.Key.Date;

            return reportGridDateView;
        }
    }
}