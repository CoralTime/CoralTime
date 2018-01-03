using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsGrandGridTimeEntryView : IReportsGrandGridView<ReportGridTimeEntryView>
    {
        public int GrandEstimatedTime { get; set; }

        public int GrandActualTime { get; set; }

        public IEnumerable<ReportGridTimeEntryView> ReportsGridView { get; set; }
    }
}
