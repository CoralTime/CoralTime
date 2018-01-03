using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsGrandGridDatesView: IReportsGrandGridView<ReportGridDateView>
    {
        public int GrandEstimatedTime { get; set; }

        public int GrandActualTime { get; set; }

        public IEnumerable<ReportGridDateView> ReportsGridView { get; set; }
    }
}
