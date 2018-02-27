using System.Collections.Generic;
using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Linq;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        public static ReportsTotalGridMembersView GetViewReportsTotalGridUsers(this ReportsTotalGridMembersView reportsGridEntitiesClients, Dictionary<Member, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridUserView = timeEntries.Select(x => x.GetViewReportGridUser(_mapper));

            foreach (var groupingTimeEntries in reportGridUserView)
            {
                reportsGridEntitiesClients.TotalActualTime += groupingTimeEntries.TotalForActualTime;
                reportsGridEntitiesClients.TotalEstimatedTime += groupingTimeEntries.TotalForEstimatedTime;
            }

            reportsGridEntitiesClients.ReportsGridView = reportGridUserView;

            return reportsGridEntitiesClients;
        }

        private static ReportTotalForGridMemberView GetViewReportGridUser(this KeyValuePair<Member, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridUserView = new ReportTotalForGridMemberView();

            SetReportsExportTotalForAndItemsValues(timeEntries, reportGridUserView, _mapper);

            reportGridUserView.MemberId = timeEntries.Key.Id;
            reportGridUserView.MemberName = timeEntries.Key.FullName;

            return reportGridUserView;
        }
    }
}