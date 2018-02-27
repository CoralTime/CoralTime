using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public interface IReportsTotalForGridItemsView
    {
        int TotalForEstimatedTime { get; set; }

        int TotalForActualTime { get; set; }

        IEnumerable<ReportsGridItemsView> Items { get; set; }
    }
}
