using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportTotalView : IReportTotalView
    {
        public ReportTotalView()
        {
            GroupedItems = new List<ReportTotalForGroupTypeView>();
            TimeTotal = new TimeTotalView();
        }

        public TimeTotalView TimeTotal { get; set; }

        public List<ReportTotalForGroupTypeView> GroupedItems { get; set; }
    }
}