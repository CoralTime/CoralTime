using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsGrandGridProjectsView: IReportsGrandGridView<ReportGridProjectView>
    {
        public int GrandEstimatedTime { get; set; }

        public int GrandActualTime { get; set; }

        public IEnumerable<ReportGridProjectView> ReportsGridView { get; set; }
    }
}
