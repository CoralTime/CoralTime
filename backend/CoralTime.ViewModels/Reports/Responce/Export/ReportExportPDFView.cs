namespace CoralTime.ViewModels.Reports.Responce.Export
{
    public class ReportExportPDFView
    {
        private ReportExportPDFView() { }

        public ReportExportPDFView(string pathContentPDFCssStyle, ReportTotalView reportTotalView)
        {
            PatchCssStyle = pathContentPDFCssStyle;
            ReportTotalView = reportTotalView;
        }

        public string PatchCssStyle { get; }

        public ReportTotalView ReportTotalView { get; set; }
    }
}
