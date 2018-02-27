using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridByDatesView: IReportsTotalGridView<ReportTotalForGridDateView>
    {
        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportTotalForGridDateView> ReportsGridView { get; set; }
    }
}
