using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public interface IReportTotalView
    {
        TimeTotalView TimeTotal { get; set; }

        List<ReportTotalForGroupTypeView> GroupedItems { get; set; }
    }
}
