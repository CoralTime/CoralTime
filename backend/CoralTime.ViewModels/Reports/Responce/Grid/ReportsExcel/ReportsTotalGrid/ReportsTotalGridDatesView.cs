using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridDatesView: IReportsTotalGridView<ReportTotalForGridDateView>
    {
        public ReportsTotalGridDatesView()
        {
            ReportsGridView = new List<ReportTotalForGridDateView>
            {
                new ReportTotalForGridDateView
                {
                    Items = new List<ReportsGridItemsView>()
                }
            };
        }

        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportTotalForGridDateView> ReportsGridView { get; set; }
    }
}
