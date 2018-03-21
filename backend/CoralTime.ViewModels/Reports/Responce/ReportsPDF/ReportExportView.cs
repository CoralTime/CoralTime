namespace CoralTime.ViewModels.Reports.PDF
{
    public class ReportExportView
    {
        private ReportExportView() { }

        public ReportExportView(string pathContentPDFCssStyle, int groupById, ReportsCell valueForPeriodCell, ReportTotalView reportTotalView)
        {
            PatchCssStyle = pathContentPDFCssStyle;
            ReportTotalView = reportTotalView;
            GroupById = groupById;
            PeriodCell = valueForPeriodCell;
            ReportTotalView = reportTotalView;
        }

        public string PatchCssStyle { get; }

        public int GroupById { get; }

        public ReportsCell PeriodCell { get; }

        public ReportTotalView ReportTotalView { get; set; }
    }
}
