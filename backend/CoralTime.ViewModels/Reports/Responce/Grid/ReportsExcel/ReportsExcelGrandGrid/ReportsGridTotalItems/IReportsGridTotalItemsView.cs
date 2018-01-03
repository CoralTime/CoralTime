using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public interface IReportsGridTotalItemsView
    {
        int TotalEstimatedTime { get; set; }

        int TotalActualTime { get; set; }

        IEnumerable<ReportsGridItemsView> Items { get; set; }
    }
}
