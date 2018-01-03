using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsGrandGridMembersView: IReportsGrandGridView<ReportGridMemberView>
    {
        public int GrandEstimatedTime { get; set; }

        public int GrandActualTime { get; set; }

        public IEnumerable<ReportGridMemberView> ReportsGridView { get; set; }
    }
}
