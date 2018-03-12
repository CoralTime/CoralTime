using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.PDF
{
    public class ReportsEntityView
    {
        public ReportsEntityView()
        {
            TotalForHeaders = new List<ReportsTotalForHeadersView>();
            EntityHeaders = new List<ReportsEntityHeadersView>();
            EntityRows = new List<List<ReportsEntityRowsView>>();
        }

        public List<ReportsTotalForHeadersView> TotalForHeaders { get; set; }

        public List<ReportsEntityHeadersView> EntityHeaders { get; set; }

        public List<List<ReportsEntityRowsView>> EntityRows { get; set; }
    }
}
