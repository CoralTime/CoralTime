using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public interface IReportsTotalGridView<T>
    {
        int TotalActualTime { get; set; }

        int TotalEstimatedTime { get; set; }

        IEnumerable<T> ReportsGridView { get; set; }
    }
}
