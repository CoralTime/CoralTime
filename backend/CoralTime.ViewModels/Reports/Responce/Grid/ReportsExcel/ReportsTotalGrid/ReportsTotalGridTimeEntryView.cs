using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridTimeEntryView : IReportsTotalGridView<ReportTotalForGridTimeEntryView>
    {
        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportTotalForGridTimeEntryView> ReportsGridView { get; set; }
    }
}
