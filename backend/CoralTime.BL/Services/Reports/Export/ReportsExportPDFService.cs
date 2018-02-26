using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.PDF;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WkWrap.Core;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        private async System.Threading.Tasks.Task<byte[]> CreateFilePDFAsync<T>(IReportsGrandGridView<T> groupedList)
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
                var pdfModel = new ReportsPDFCommonView(pathContentPDFCssStyle, GroupById, GetPeriodPDFCell())
                {
                    PDFGrandModel = CreateReportsGrandGridModel(groupedList)
                };

                #region Parse view.

                var engine = new RazorLightEngineBuilder()
                              .UseFilesystemProject(pathContentPDF)
                              .UseMemoryCachingProvider()
                              .Build();

                var htmlFromParsedViewRazorLight = await engine.CompileRenderAsync(fileNamePDFMarkUpView, pdfModel);

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

        private ReportsPDFGrandGridView CreateReportsGrandGridModel<T>(IReportsGrandGridView<T> groupedList)
        {
            var pdfGrandModel = new ReportsPDFGrandGridView();

            // Grand headers.
            foreach (var propGrand in PropsEntitiesTotalHeaders)
            {
                if (!propGrand.PropertyType.GetTypeInfo().IsGenericType)
                {
                    var grandHeader = CreateGrandHeader(propGrand, groupedList);
                    pdfGrandModel.GrandHeaders.Add(grandHeader);
                }
            }

            foreach (var entity in groupedList.ReportsGridView.ToList())
            {
                var entityLocal = new ReportsPDFEntityView();

                foreach (var entityHeader in PropsGroupByAndTotalTimes)
                {
                    if (!entityHeader.PropertyType.GetTypeInfo().IsGenericType)
                    {
                        // Total headers.
                        var totalHeader = CreateTotalHeader(entityHeader, entity);
                        entityLocal.TotalHeaders.Add(totalHeader);
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

                pdfGrandModel.Entities.Add(entityLocal);
            }

            return pdfGrandModel;
        }

        private ReportsPDFGrandHeaders CreateGrandHeader<T>(PropertyInfo prop, T entity)
        {
            var grandEntityHeadersModel = CreateCell<T, ReportsPDFGrandHeaders>(prop, entity);
            return grandEntityHeadersModel;
        }

        private ReportsPDFTotalHeadersView CreateTotalHeader<T>(PropertyInfo prop, T entity)
        {
            var entityHeaderCell = CreateCell<T, ReportsPDFTotalHeadersView>(prop, entity);
            return entityHeaderCell;
        }

        private List<ReportsPDFEntityHeadersView> CreateEntityHeaders()
        {
            var entitiesHeaders = new List<ReportsPDFEntityHeadersView>();
            
            foreach (var availableProp in ExludePropByDefaultGrouping(PropsEntityHeadersAndRows))
            {
                var entityHeaderCell = CreateCell<object, ReportsPDFEntityHeadersView>(availableProp, null);
                entitiesHeaders.Add(entityHeaderCell);
            }

            return entitiesHeaders;
        }

        private List<List<ReportsPDFEntityRowsView>> CreateEntityRows<T>(PropertyInfo entityHeader, T entity)
        {
            var entitiesRowsList = new List<List<ReportsPDFEntityRowsView>>();

            foreach (var entityRow in GetValueListFromProp(entityHeader, entity))
            {
                var entityRowsLocal = new List<ReportsPDFEntityRowsView>();

                foreach (var prop in PropsEntityHeadersAndRows)
                {
                    if (!IsPropByDefaultGrouping(prop.Name))
                    {
                        var entityRowCell = CreateCell<ReportsGridItemsView, ReportsPDFEntityRowsView>(prop, entityRow);
                        entityRowsLocal.Add(entityRowCell);
                    }
                }

                entitiesRowsList.Add(entityRowsLocal);
            }

            return entitiesRowsList;
        }
    }
}