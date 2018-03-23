using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportTotalForView
    {
        public ReportTotalForView()
        {
            TimeTotalFor = new TimeTotalForView();
            Items = new List<ReportItemsView>();
        }

        public TimeTotalForView TimeTotalFor { get; set; }

        public List<ReportItemsView> Items { get; set; }
    }
}