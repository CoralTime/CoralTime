using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.PDF
{
    public class ReportsPDFEntityView
    {
        public ReportsPDFEntityView()
        {
            TotalHeaders = new List<ReportsPDFTotalHeadersView>();
            EntityHeaders = new List<ReportsPDFEntityHeadersView>();
            EntityRows = new List<List<ReportsPDFEntityRowsView>>();
        }

        public List<ReportsPDFTotalHeadersView> TotalHeaders { get; set; }

        public List<ReportsPDFEntityHeadersView> EntityHeaders { get; set; }

        public List<List<ReportsPDFEntityRowsView>> EntityRows { get; set; }
    }
}
