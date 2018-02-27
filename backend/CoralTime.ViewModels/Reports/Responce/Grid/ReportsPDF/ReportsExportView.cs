namespace CoralTime.ViewModels.Reports.PDF
{
    public class ReportsExportView
    {
        private ReportsExportView() { }

        public ReportsExportView(string pathContentPDFCssStyle, int groupById, ReportsCell valueForPeriodCell)
        {
            PatchCssStyle = pathContentPDFCssStyle;
            ReportsTotalView = new ReportsTotalView();
            GroupById = groupById;
            PeriodCell = valueForPeriodCell;
        }

        public string PatchCssStyle { get; }

        public int GroupById { get; }

        public ReportsCell PeriodCell { get; }

        public ReportsTotalView ReportsTotalView { get; set; }
    }
}
