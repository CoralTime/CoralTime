using AutoMapper;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ReportsTotalGridClientsView GetViewReportsTotalGridClients(this ReportsTotalGridClientsView reportsGridEntitiesClients, Dictionary<Client, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportTotalClientView = timeEntries.Select(x => x.GetViewReportGridClientView(_mapper));

            foreach (var groupingTimeEntries in reportTotalClientView)
            {
                reportsGridEntitiesClients.TotalActualTime += groupingTimeEntries.TotalForActualTime;
                reportsGridEntitiesClients.TotalEstimatedTime += groupingTimeEntries.TotalForEstimatedTime;
            }

            reportsGridEntitiesClients.ReportsGridView = reportTotalClientView;

            return reportsGridEntitiesClients;
        }

        private static ReportsTotalForGridClientView GetViewReportGridClientView(this KeyValuePair<Client, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridClientView = new ReportsTotalForGridClientView();

            SetReportsExportTotalForAndItemsValues(timeEntries, reportGridClientView, _mapper);

            reportGridClientView.ClientId = timeEntries.Key.Id;
            reportGridClientView.ClientName = timeEntries.Key.Id == Constants.WithoutClient.Id ? Constants.WithoutClient.Name : timeEntries.Key.Name;

            return reportGridClientView;
        }
    }
}