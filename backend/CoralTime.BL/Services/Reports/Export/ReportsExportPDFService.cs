using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.PDF;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WkWrap.Core;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        private async Task<byte[]> CreateFilePDFAsync<T>(IReportsTotalGridView<T> groupedList)
        {
            if (!RunSetCommonValuesForExport)
            {
                throw new InvalidOperationException("You forgot run SetCommonValuesForExport() for set common values.");
            }

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
                var reportsExportView = new ReportsExportView(pathContentPDFCssStyle, GroupById, GetPeriodPDFCell())
                {
                    ReportsTotalView = CreateReportsExportView(groupedList)
                };

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

        private ReportsTotalView CreateReportsExportView<T>(IReportsTotalGridView<T> groupedList)
        {
            var totalModel = new ReportsTotalView();

            // Total headers.
            foreach (var propTotal in PropsEntitiesTotalHeaders)
            {
                if (!propTotal.PropertyType.GetTypeInfo().IsGenericType)
                {
                    var totalHeader = CreateTotalHeader(propTotal, groupedList);
                    totalModel.TotalHeaders.Add(totalHeader);
                }
            }

            foreach (var entity in groupedList.ReportsGridView.ToList())
            {
                var entityLocal = new ReportsEntityView();

                foreach (var entityHeader in PropsGroupByAndTotalTimes)
                {
                    if (!entityHeader.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        // Total header.
                        var totalHeader = CreateTotalForHeade(entityHeader, entity);
                        entityLocal.TotalForHeaders.Add(totalHeader);
                    }
                    else if (entityHeader.PropertyType == typeof(IEnumerable<ReportsGridItemsView>))
                    {
                        // Entity Headers.
                        var entityHeaders = CreateEntityHeaders();
                        entityLocal.EntityHeaders.AddRange(entityHeaders);

                        // Entity Rows.
                        var entityRows = CreateEntityRows(entityHeader, entity);
                        entityLocal.EntityRows.AddRange(entityRows);
                    }
                }

                totalModel.Entities.Add(entityLocal);
            }

            return totalModel;
        }

        private ReportsTotalHeadersView CreateTotalHeader<T>(PropertyInfo prop, T entity)
        {
            var grandEntityHeadersModel = CreateCell<T, ReportsTotalHeadersView>(prop, entity);
            return grandEntityHeadersModel;
        }

        private ReportsTotalForHeadersView CreateTotalForHeade<T>(PropertyInfo prop, T entity)
        {
            var entityHeaderCell = CreateCell<T, ReportsTotalForHeadersView>(prop, entity);
            return entityHeaderCell;
        }

        private List<ReportsEntityHeadersView> CreateEntityHeaders()
        {
            var entitiesHeaders = new List<ReportsEntityHeadersView>();
            
            foreach (var availableProp in ExludePropByDefaultGrouping(PropsEntityHeadersAndRows))
            {
                var entityHeaderCell = CreateCell<object, ReportsEntityHeadersView>(availableProp, null);
                entitiesHeaders.Add(entityHeaderCell);
            }

            return entitiesHeaders;
        }

        private List<List<ReportsEntityRowsView>> CreateEntityRows<T>(PropertyInfo entityHeader, T entity)
        {
            var entitiesRowsList = new List<List<ReportsEntityRowsView>>();

            foreach (var entityRow in GetValueListFromProp(entityHeader, entity))
            {
                var entityRowsLocal = new List<ReportsEntityRowsView>();

                foreach (var prop in PropsEntityHeadersAndRows)
                {
                    if (!IsGroupByThisProperty(prop.Name))
                    {
                        var entityRowCell = CreateCell<ReportsGridItemsView, ReportsEntityRowsView>(prop, entityRow);
                        entityRowsLocal.Add(entityRowCell);
                    }
                }

                entitiesRowsList.Add(entityRowsLocal);
            }

            return entitiesRowsList;
        }
    }
}