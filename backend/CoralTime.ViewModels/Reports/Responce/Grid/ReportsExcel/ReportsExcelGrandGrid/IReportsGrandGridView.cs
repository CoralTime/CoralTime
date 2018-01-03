using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public interface IReportsGrandGridView<T>
    {
        int GrandActualTime { get; set; }

        int GrandEstimatedTime { get; set; }

        IEnumerable<T> ReportsGridView { get; set; }
    }
}
