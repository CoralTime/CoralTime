using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsGridTotalItemsView : IReportsGridTotalItemsView
    {
        protected ReportsGridTotalItemsView()
        {
            Items = new List<ReportsGridItemsView>();
        }

        public int TotalActualTime { get; set; }

        public int TotalEstimatedTime { get; set; }

        public IEnumerable<ReportsGridItemsView> Items { get; set; }
    }
}