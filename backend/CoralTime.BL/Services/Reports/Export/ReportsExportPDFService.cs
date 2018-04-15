using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Responce.Export;
using RazorLight;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WkWrap.Core;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        private async Task<byte[]> CreateFilePDFAsync(ReportTotalView reportTotalView)
        {
            var pdfBytesResult = new byte[0];

            #region Set root paths and give names for files.

            var fileNameWkhtmltopdf = "wkhtmltopdf.exe";
            var fileNamePDFMarkUpView = "PDFMarkUpView.cshtml";

            var contentRootPath = _environment.ContentRootPath;

            var pathContentPDF = $"{contentRootPath}\\Content\\PDF";
            var pathContentPDFCssStyle = $"{pathContentPDF}\\Site.css";
            var pathContentPDFWkhtmltopdf = $"{pathContentPDF}\\{fileNameWkhtmltopdf}";

            var pathFileInfo = new FileInfo(pathContentPDFWkhtmltopdf);

            #endregion

            if (File.Exists(pathContentPDFWkhtmltopdf))
            {
                var reportsExportView = new ReportExportPDFView(pathContentPDFCssStyle, /*GetPeriodPDFCell(),*/ reportTotalView);

                #region Parse view.

                var engine = new RazorLightEngineBuilder()
                              .UseFilesystemProject(pathContentPDF)
                              .UseMemoryCachingProvider()
                              .Build();

                var htmlFromParsedViewRazorLight = await engine.CompileRenderAsync(fileNamePDFMarkUpView, reportsExportView);

                var settings = new ConversionSettings(
                    pageSize: PageSize.A4,
                    orientation: PageOrientation.Landscape,
                    margins: new WkWrap.Core.PageMargins(5, 10, 5, 10),
                    grayscale: false,
                    lowQuality: false,
                    quiet: false,
                    enableJavaScript: true,
                    javaScriptDelay: null,
                    enableExternalLinks: true,
                    enableImages: true,
                    executionTimeout: null);

                pdfBytesResult = new HtmlToPdfConverter(pathFileInfo).ConvertToPdf(htmlFromParsedViewRazorLight, Encoding.UTF8, settings);

                #endregion
            }

            return pdfBytesResult;
        }
    }
}