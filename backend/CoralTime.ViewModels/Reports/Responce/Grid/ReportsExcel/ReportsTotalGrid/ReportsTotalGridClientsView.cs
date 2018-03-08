using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridClientsView : IReportsTotalGridView<ReportsTotalForGridClientView>
    {
        public ReportsTotalGridClientsView()
        {
            ReportsGridView = new List<ReportsTotalForGridClientView>
            {
                new ReportsTotalForGridClientView
                {
                    Items = new List<ReportsGridItemsView>()
                }
            };
        }

        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportsTotalForGridClientView> ReportsGridView { get; set; }
    }
}
