using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridProjectsView: IReportsTotalGridView<ReportTotalForGridProjectView>
    {
        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportTotalForGridProjectView> ReportsGridView { get; set; }
    }
}
