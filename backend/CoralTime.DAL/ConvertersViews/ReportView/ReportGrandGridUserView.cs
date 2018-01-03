using System.Collections.Generic;
using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Linq;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ReportsGrandGridMembersView GetViewReportsGrandGridClients(this ReportsGrandGridMembersView reportsGridEntitiesClients, Dictionary<Member, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridUserView = timeEntries.Select(x => x.GetViewReportGridUser(_mapper));

            foreach (var groupingTimeEntries in reportGridUserView)
            {
                reportsGridEntitiesClients.GrandActualTime += groupingTimeEntries.TotalActualTime;
                reportsGridEntitiesClients.GrandEstimatedTime += groupingTimeEntries.TotalEstimatedTime;
            }

            reportsGridEntitiesClients.ReportsGridView = reportGridUserView;

            return reportsGridEntitiesClients;
        }

        private static ReportGridMemberView GetViewReportGridUser(this KeyValuePair<Member, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridUserView = new ReportGridMemberView();

            SetReportsGridItemViewValues(timeEntries, reportGridUserView, _mapper);

            reportGridUserView.MemberId = timeEntries.Key.Id;
            reportGridUserView.MemberName = timeEntries.Key.FullName;

            return reportGridUserView;
        }
    }
}