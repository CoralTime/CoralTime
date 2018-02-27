using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalForGridItemsView : IReportsTotalForGridItemsView
    {
        protected ReportsTotalForGridItemsView()
        {
            Items = new List<ReportsGridItemsView>();
        }

        public int TotalForActualTime { get; set; }

        public int TotalForEstimatedTime { get; set; }

        public IEnumerable<ReportsGridItemsView> Items { get; set; }
    }
}