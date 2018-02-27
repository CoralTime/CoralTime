using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridClients : IReportsTotalGridView<ReportsTotalForGridClientView>
    {
        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportsTotalForGridClientView> ReportsGridView { get; set; }
    }
}
