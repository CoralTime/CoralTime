using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsGrandGridClients : IReportsGrandGridView<ReportGridClientView>
    {
        public int GrandEstimatedTime { get; set; }

        public int GrandActualTime { get; set; }

        public IEnumerable<ReportGridClientView> ReportsGridView { get; set; }
    }
}
