using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.PDF
{
    public class ReportsPDFGrandGridView
    {
        public ReportsPDFGrandGridView()
        {
            GrandHeaders = new List<ReportsPDFGrandHeaders>();
            Entities = new List<ReportsPDFEntityView>();
        }

        public List<ReportsPDFGrandHeaders> GrandHeaders { get; set; }

        public List<ReportsPDFEntityView> Entities { get; set; }
    }
}