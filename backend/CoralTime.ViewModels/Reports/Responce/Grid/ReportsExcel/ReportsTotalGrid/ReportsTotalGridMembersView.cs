using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridMembersView: IReportsTotalGridView<ReportTotalForGridMemberView>
    {
        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportTotalForGridMemberView> ReportsGridView { get; set; }
    }
}
