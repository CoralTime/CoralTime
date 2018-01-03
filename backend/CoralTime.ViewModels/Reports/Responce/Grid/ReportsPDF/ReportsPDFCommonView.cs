namespace CoralTime.ViewModels.Reports.PDF
{
    public class ReportsPDFCommonView
    {
        private ReportsPDFCommonView() { }

        public ReportsPDFCommonView(string pathContentPDFCssStyle, int groupById, PDFCell valueForPeriodCell)
        {
            PatchCssStyle = pathContentPDFCssStyle;
            PDFGrandModel = new ReportsPDFGrandGridView();
            GroupById = groupById;
            PeriodCell = valueForPeriodCell;
        }

        public string PatchCssStyle { get; }

        public int GroupById { get; }

        public PDFCell PeriodCell { get; }

        public ReportsPDFGrandGridView PDFGrandModel { get; set; }
    }
}
