using AutoMapper;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ReportsGrandGridClients GetViewReportsGrandGridClients(this ReportsGrandGridClients reportsGridEntitiesClients, Dictionary<Client, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridClientView = timeEntries.Select(x => x.GetViewReportGridClientView(_mapper));

            foreach (var groupingTimeEntries in reportGridClientView)
            {
                reportsGridEntitiesClients.GrandActualTime += groupingTimeEntries.TotalActualTime;
                reportsGridEntitiesClients.GrandEstimatedTime += groupingTimeEntries.TotalEstimatedTime;
            }

            reportsGridEntitiesClients.ReportsGridView = reportGridClientView;

            return reportsGridEntitiesClients;
        }

        private static ReportGridClientView GetViewReportGridClientView(this KeyValuePair<Client, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridClientView = new ReportGridClientView();

            SetReportsGridItemViewValues(timeEntries, reportGridClientView, _mapper);

            reportGridClientView.ClientId = timeEntries.Key.Id;
            reportGridClientView.ClientName = timeEntries.Key.Id == Constants.WithoutClient.Id ? Constants.WithoutClient.Name : timeEntries.Key.Name;

            return reportGridClientView;
        }
    }
}