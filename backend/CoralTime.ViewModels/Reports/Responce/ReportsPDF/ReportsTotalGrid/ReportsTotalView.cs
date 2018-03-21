using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.PDF
{
    public class ReportsTotalView
    {
        public ReportsTotalView()
        {
            TotalHeaders = new List<ReportsTotalHeadersView>();
            Entities = new List<ReportsEntityView>();
        }

        public List<ReportsTotalHeadersView> TotalHeaders { get; set; }

        public List<ReportsEntityView> Entities { get; set; }
    }
}