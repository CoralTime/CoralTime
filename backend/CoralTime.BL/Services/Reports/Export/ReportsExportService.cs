using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.PDF;
using CoralTime.ViewModels.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Responce.ReportsGrid;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService : BaseService, IReportExportService
    {
        private IHostingEnvironment _environment { get; }
        private readonly IConfiguration _configuration;
        private readonly IReportsService _reportService;

        public ReportsExportService(UnitOfWork uow, IMapper mapper, IConfiguration configuration, IHostingEnvironment environment, IReportsService reportService)
            : base(uow, mapper)
        {
            _configuration = configuration;
            _environment = environment;
            _reportService = reportService;
        }

        #region Constants.

        private const string ExtensionXLSX = ".xlsx";
        private const string ExtensionCSV = ".csv";
        private const string ExtensionPDF = ".pdf";

        private const string ContentTypeXLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //private const string ContentTypeXLSX = "application/octet-stream";
        private const string ContentTypeCSV = "application/csv";
        private const string ContentTypePDF = "application/pdf";

        private enum FileType
        {
            Excel = 0,
            CSV = 1,
            PDF = 2
        }

        #endregion

        #region Properties. 

        private bool RunSetCommonValuesForExport { get; set; }

        private string FileName { get; set; } = Constants.CoralTime;

        private string ContentType { get; set; } = string.Empty;

        private DateTime DateFrom { get; set; }

        private DateTime DateTo { get; set; }

        private int GroupById { get; set; }

        private int[] ShowColumnIds { get; set; }

        private int DateFormatId { get; set; }

        private int CountPeriodCells { get; set; }

        private int CountGroupByCells { get; } = 1;

        #endregion

        #region Exclude excess properties (Exclude enums and array of Properties) 

        public enum ExternalProperties
        {
            ProjectName,
            ClientName,
            MemberName,
            Date,
            TotalForActualTime,
            TotalForEstimatedTime,
            TotalActualTime,
            TotalEstimatedTime,
        }

        public enum InternalProperties
        {
            TimeEntryName,
            ProjectName,
            ClientName,
            MemberName,
            TaskName,
            Date,
            TimeFrom,
            TimeTo,
            ActualTime,
            EstimatedTime,
            Description,
            TotalForActualTime,
            TotalForEstimatedTime,
        }

        private readonly List<string> _alwaysShowProperty = new List<string>
        {
            InternalProperties.ProjectName.ToString(),
            InternalProperties.MemberName.ToString(),
            InternalProperties.TaskName.ToString()
        };

        private enum ExcludePropertyByDefault
        {
            TimeEntryId,
            ProjectId,
            ClientId,
            MemberId,
            TaskId,
        }
        
        private string[] _excludeProperties =
        {
            ExcludePropertyByDefault.TimeEntryId.ToString(),
            ExcludePropertyByDefault.ProjectId.ToString(),
            ExcludePropertyByDefault.ClientId.ToString(),
            ExcludePropertyByDefault.MemberId.ToString(),
            ExcludePropertyByDefault.TaskId.ToString(),
        };

        public enum ShowColumnModelIds
        {
            ShowEstimatedTime = 1 ,
            ShowDate = 2,
            ShowNotes = 3,
            ShowStartFinish = 4
        }

        public static readonly ShowColumnModel[] showColumnsInfo =
        {
            //new ShowColumnModel{ Id = 0, ShowColumnDescriptions = new List<ShowColumnDescription>
            //{
            //    new ShowColumnDescription { Name = InternalProperties.ActualTime.ToString(), Description = "Show Actual Hours" },
            //    new ShowColumnDescription { Name = InternalProperties.TotalActualTime.ToString(), Description = "Show Total Actual Hours" },
            //    new ShowColumnDescription { Name = ExternalProperties.GrandActualTime.ToString(), Description = "Show Grand Actual Hours" },
            //}},
            new ShowColumnModel{ Id = (int) ShowColumnModelIds.ShowEstimatedTime, ShowColumnDescriptions = new List<ShowColumnDescription>
            {
                new ShowColumnDescription { Name = InternalProperties.EstimatedTime.ToString(), Description = "Show Estimated Hours" },
                new ShowColumnDescription { Name = InternalProperties.TotalForEstimatedTime.ToString(), Description = "Show Total Estimated Hours" },
                new ShowColumnDescription { Name = ExternalProperties.TotalEstimatedTime.ToString(), Description = "Show Grand Estimated Hours" },
            }},
            new ShowColumnModel{ Id = (int) ShowColumnModelIds.ShowDate, ShowColumnDescriptions = new List<ShowColumnDescription>
            {
                new ShowColumnDescription { Name = InternalProperties.Date.ToString(), Description = "Show Date"}
            }},
            new ShowColumnModel{ Id = (int) ShowColumnModelIds.ShowNotes, ShowColumnDescriptions = new List<ShowColumnDescription>
            {
                new ShowColumnDescription { Name = InternalProperties.Description.ToString(), Description = "Show Notes"}
            }},
            new ShowColumnModel{ Id = (int) ShowColumnModelIds.ShowStartFinish, ShowColumnDescriptions = new List<ShowColumnDescription>
            {
                new ShowColumnDescription { Name = InternalProperties.TimeFrom.ToString(), Description = "Show Start Time"},
                new ShowColumnDescription { Name = InternalProperties.TimeTo.ToString(), Description = "Show Finish Time"}
            }}
        };

        #endregion

        #region Export Excel, CSV, PDF. 

        public async Task<FileResult> ExportFileGroupByProjectsAsync(ReportsGridView reportsGridData, HttpContext httpContext)
        {
            var groupByProjects = _reportService.GetGroupingReportsGridByProjects(reportsGridData);
            var exportFileGroupByProjects = await GetGroupedReportsExportFileAsync(reportsGridData, httpContext, groupByProjects);

            return exportFileGroupByProjects;
        }

        public async Task<FileResult> ExportFileGroupByUsersAsync(ReportsGridView reportsGridData, HttpContext httpContext)
        {
            var groupByUsers = _reportService.GetGroupingReportsGridByUsers(reportsGridData);
            var exportFileGroupByUsers = await GetGroupedReportsExportFileAsync(reportsGridData, httpContext, groupByUsers);

            return exportFileGroupByUsers;
        }

        public async Task<FileResult> ExportFileGroupByDatesAsync(ReportsGridView reportsGridData, HttpContext httpContext)
        {
            var groupByDates = _reportService.GetGroupingReportsGridByDates(reportsGridData);
            var exportFileGroupByDates = await GetGroupedReportsExportFileAsync(reportsGridData, httpContext, groupByDates);

            return exportFileGroupByDates;
        }

        public async Task<FileResult> ExportFileGroupByClientsAsync(ReportsGridView reportsGridData, HttpContext httpContext)
        {
            var groupByClient = _reportService.GetGroupingReportsGridByClients(reportsGridData);
            var exportFileGroupByClients = await GetGroupedReportsExportFileAsync(reportsGridData, httpContext, groupByClient);

            return exportFileGroupByClients;
        }

        #endregion

        #region Export Excel, CSV, PDF. (Common methods)

        private async Task<FileStreamResult> GetGroupedReportsExportFileAsync<T>(ReportsGridView reportsGridData, HttpContext httpContext, IReportsTotalGridView<T> groupingList)
        {
            var fileOfBytes = await CreateReportsFileOfBytesAsync(reportsGridData, groupingList);
            var fileStreamResult = SaveFileToFileStreamResult(httpContext, fileOfBytes);

            return fileStreamResult;
        }

        //private DataSet ConvertListToDataSet<T>(IReportsExportView<T> groupingList)
        //{
        //    var dataSet = new DataSet(GetValueForCellPeriodDate());

        //    foreach (var reportsGridView in groupingList.ReportsGridView)
        //    {
        //        var datatable = new DataTable();

        //        foreach (var propGroupByAndTotalTime in PropsGroupByAndTotalTimes)
        //        {
        //            var propType = propGroupByAndTotalTime.PropertyType;

        //            if (!propType.GetTypeInfo().IsGenericType && IsGroupByThisProperty(propGroupByAndTotalTime.Name))
        //            {
        //                var valueSingleFromProp = GetValueSingleFromProp(propGroupByAndTotalTime, reportsGridView);
        //                var tableNameFromGroupByHeader = CreateTableNameFromGroupByHeader(valueSingleFromProp, propGroupByAndTotalTime);

        //                datatable.TableName = tableNameFromGroupByHeader;
        //            }
        //            else if (propType.GetTypeInfo().IsGenericType)
        //            {
        //                if (propType == typeof(IEnumerable<ReportsGridItemsView>))
        //                {
        //                    // Row Entity Header Names.
        //                    CreateRowOfEntityHeaderNames(datatable);

        //                    // Rows Entity Header Values.
        //                    var valueListFromProp = GetValueListFromProp(propGroupByAndTotalTime, reportsGridView);
        //                    CreateRowsEntityValues(datatable, valueListFromProp);
        //                }
        //            }
        //        }

        //        dataSet.Tables.Add(datatable);
        //    }

        //    return dataSet;
        //}

        //private void CreateRowOfEntityHeaderNames(DataTable datatable)
        //{
        //    var nestedEntityHeaders = RenameNestedEntityHeaders(PropsEntityHeadersAndRows);

        //    foreach (var nestedEntityHeader in nestedEntityHeaders)
        //    {
        //        datatable.Columns.Add(new DataColumn(nestedEntityHeader));
        //    }
        //}

        private async Task<byte[]> CreateReportsFileOfBytesAsync<T>(ReportsGridView reportsGridData, IReportsTotalGridView<T> groupingList)
        {
            SetCommonValuesForExport<T>(reportsGridData);

            //var dataSet = ConvertListToDataSet(groupingList);

            var fileOfBytes = new byte[0];

            UpdateFileName();

            switch (reportsGridData.FileTypeId ?? 0)
            {
                case (int) FileType.Excel:
                {
                    FileName = FileName + ExtensionXLSX;
                    //file = CreateFileExcel(dataSet);
                    ContentType = ContentTypeXLSX;

                    break;
                }

                case (int) FileType.CSV:
                {
                    FileName = FileName + ExtensionCSV;
                    //file = CreateFileCSV(dataSet);
                    ContentType = ContentTypeCSV;

                    break;
                }

                case (int) FileType.PDF:
                {
                    FileName = FileName + ExtensionPDF;
                    fileOfBytes = await CreateFilePDFAsync(groupingList);
                    ContentType = ContentTypePDF;

                    break;
                }
            }

            return fileOfBytes;
        }

        private FileStreamResult SaveFileToFileStreamResult(HttpContext httpContext, byte[] fileByte)
        {
            httpContext.Response.ContentType = ContentType;

            var fileStreamResult = new FileStreamResult(new MemoryStream(fileByte), new MediaTypeHeaderValue(ContentType))
            {
                FileDownloadName = FileName
            };

            return fileStreamResult;
        }

        private void UpdateFileName()
        {
            if (!string.IsNullOrEmpty(_reportService.SingleFilteredProjectName))
            {
                FileName = FileName + " " + _reportService.SingleFilteredProjectName + " " + GetAbbreviatedMonthName(DateFrom) + " - " + GetAbbreviatedMonthName(DateTo);
            }
            else
            {
                FileName = FileName + " Reports " + GetAbbreviatedMonthName(DateFrom) + " - " + GetAbbreviatedMonthName(DateTo);
            }
        }

        private string GetAbbreviatedMonthName(DateTime date)
        {
            return CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month) + " " + date.Day;
        }

        private void SetCommonValuesForExport<T>(ReportsGridView reportsGridData)
        {
            RunSetCommonValuesForExport = true;

            #region Set Global Properties. 
            // TODO change type!
            GroupById = reportsGridData.CurrentQuery.GroupById ?? 3;

            ShowColumnIds = reportsGridData.CurrentQuery.ShowColumnIds;

            DateFormatId = reportsGridData.DateFormatId;

            DateFrom = _reportService.DateFrom;
            DateTo = _reportService.DateTo;

            #endregion

            #region Get excluded props by Grand, Entity, NestedEntity headers in arrays.

            PropsGroupByAndTotalTimes = ExcludeProps(typeof(T));
            PropsEntityHeadersAndRows = ExcludeProps(typeof(IReportsGridItemsView));
            PropsEntitiesTotalHeaders = ExcludeProps(typeof(IReportsTotalGridView<T>));

            #endregion
        }

        #region Exclude excess properties

        /*
            1. Exlude one field ex:"ClientId" in external headers - ExcludeExternalPropInDependenceByGrouping() add field in Array "string[] _excludeProperties" from "enum InternalProperties".
            2. Exclude plural fields (Ids) in grid - ExcludeProps(): add field in Array "string[] _excludeProperties" from "enum ExcludePropertyByDefault"
            3. Exclude by custom options - ExcludeProps():           add field in Array "string[] _excludeProperties" from "enum ExternalProperties" AND "enum InternalProperties"
            4. Rename some properties.
        */

        private bool IsGroupByThisProperty(string propName)
        {
            var result = propName == InternalProperties.ProjectName.ToString() && GroupById == (int) Constants.ReportsGroupBy.Project
                         || propName == InternalProperties.MemberName.ToString() && GroupById == (int) Constants.ReportsGroupBy.User
                         || propName == InternalProperties.Date.ToString() && GroupById == (int) Constants.ReportsGroupBy.Date
                         || propName == InternalProperties.ClientName.ToString() && GroupById == (int) Constants.ReportsGroupBy.Client
                         || propName == InternalProperties.TimeEntryName.ToString() && GroupById == (int) Constants.ReportsGroupBy.None;

            return result;
        }

        private bool IsPropTotalOrActualTime(string propName)
        {
            var result = propName == InternalProperties.TotalForActualTime.ToString() 
                         || propName == InternalProperties.TotalForEstimatedTime.ToString();
            return result;
        }

        private List<PropertyInfo> ExcludeProps(Type type)
        {
            var hideColumns = AddHideColumns();

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
   
            var availableProps = new List<PropertyInfo>();
            foreach (var prop in props)
            {
                if (hideColumns.All(x => x != prop.Name))
                {
                    availableProps.Add(prop);
                }
            }

            return availableProps;
        }

        private List<PropertyInfo> ExcludePropsEntity(Type type)
        {
            var hideColumns = AddHideColumns();

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            var availableProps = new List<PropertyInfo>();
            foreach (var prop in props)
            {
                if (hideColumns.All(x => x != prop.Name) && prop.Name != ExternalProperties.TotalForActualTime.ToString() && prop.Name != ExternalProperties.TotalForEstimatedTime.ToString())
                {
                    availableProps.Add(prop);
                }
            }

            return availableProps;
        }

        private string[] AddHideColumns()
        {
            if (ShowColumnIds != null)
            {
                foreach (var showColumnInfo in showColumnsInfo)
                {
                    var isAddPropToExcludeArray = !ShowColumnIds.Contains(showColumnInfo.Id);
                    if (isAddPropToExcludeArray)
                    {
                        var showColumnDescriptionsById = showColumnsInfo.FirstOrDefault(x => x.Id == showColumnInfo.Id)?.ShowColumnDescriptions;
                        
                        foreach (var tmpItem in showColumnDescriptionsById)
                        {
                            if (GroupById == (int)Constants.ReportsGroupBy.Date && showColumnDescriptionsById.Any(x => x.Name.Contains(InternalProperties.Date.ToString())))
                            {
                                continue;
                            }

                            _excludeProperties = _excludeProperties.Append(tmpItem.Name).ToArray();
                        }
                    }
                }
            }

            return _excludeProperties;
        }

        private List<string> RenameNestedEntityHeaders(List<PropertyInfo> props)
        {
            var result = new List<string>();

            foreach (var prop in props)
            {
                if (IsGroupByThisProperty(prop.Name))
                {
                    continue;
                }

                if (prop.Name == InternalProperties.ClientName.ToString())
                {
                    result.Add("Client");
                    continue;
                }

                if (prop.Name == InternalProperties.MemberName.ToString())
                {
                    result.Add("User");
                    continue;
                }

                if (prop.Name == InternalProperties.ProjectName.ToString())
                {
                    result.Add("Project");
                    continue;
                }

                if (prop.Name == InternalProperties.TaskName.ToString())
                {
                    result.Add("Task");
                    continue;
                }

                if (prop.Name == InternalProperties.Description.ToString())
                {
                    result.Add("Notes");
                    continue;
                }

                if (prop.Name == InternalProperties.ActualTime.ToString())
                {
                    result.Add("Act. Hours");
                    continue;
                }

                if (prop.Name == InternalProperties.EstimatedTime.ToString())
                {
                    result.Add("Est. Hours");
                    continue;
                }

                if (prop.Name == InternalProperties.TimeFrom.ToString())
                {
                    result.Add("Start");
                    continue;
                }

                if (prop.Name == InternalProperties.TimeTo.ToString())
                {
                    result.Add("Finish");
                    continue;
                }

                result.Add(prop.Name);
            }

            return result;
        }

        public enum NameDisplays
        {
            Notes
        }

        private string GetNameDisplayForGrandAndTotalHeaders(string propName)
        {
            if (propName == ExternalProperties.TotalActualTime.ToString())
            {
                return string.Empty;
            }

            if (propName == ExternalProperties.TotalEstimatedTime.ToString())
            {
                return string.Empty;
            }

            if (propName == InternalProperties.TotalForActualTime.ToString())
            {
                return "TOTAL FOR: ";
            }

            if (propName == InternalProperties.TotalForEstimatedTime.ToString())
            {
                return "TOTAL FOR: ";
            }

            if (propName == InternalProperties.TimeEntryName.ToString())
            {
                return string.Empty;
            }

            if (propName == InternalProperties.ProjectName.ToString())
            {
                var tmpName = "Project";

                if (GroupById == (int)Constants.ReportsGroupBy.Project)
                {
                    tmpName = tmpName.ToUpper() + ": ";
                }

                return tmpName;
            }

            if (propName == ExternalProperties.Date.ToString())
            {
                return "DATE: ";
            }

            if (propName == InternalProperties.MemberName.ToString())
            {
                var tmpName = "User";

                if (GroupById == (int)Constants.ReportsGroupBy.User)
                {
                    tmpName = tmpName.ToUpper() + ": ";
                }

                return tmpName;
            }

            if (propName == InternalProperties.ClientName.ToString())
            {
                var tmpName = "Client";

                if (GroupById == (int)Constants.ReportsGroupBy.Client)
                {
                    tmpName = tmpName.ToUpper() + ": ";
                }

                return tmpName;
            }

            if (propName == InternalProperties.Date.ToString())
            {
                var tmpName = "Date";

                if (GroupById == (int)Constants.ReportsGroupBy.User)
                {
                    tmpName = InternalProperties.Date.ToString().ToUpper() + ": ";
                }

                return tmpName;
            }

            /*
            if (GroupById == (int)Constants.ReportsGroupBy.User)
            {
                PDFcell.NameDisplay = PDFcell.NameDisplay + ": ";
            }
            */

            return propName;
        }

        private List<PropertyInfo> ExludePropByDefaultGrouping(List<PropertyInfo> props)
        {
            var availableProps = new List<PropertyInfo>();

            foreach (var prop in props)
            {
                if (IsGroupByThisProperty(prop.Name))
                {
                    continue;
                }

                availableProps.Add(prop);
            }

            return availableProps;
        }

        private ReportsCell GetNameDisplayForTotalHeaderPDF(PropertyInfo prop, string value)
        {
            var PDFcell = new ReportsCell
            {
                NameDisplay = prop.Name,
                NameDefault = prop.Name
            };

            #region Total headers 

            if (prop.Name == ExternalProperties.TotalActualTime.ToString())
            {
                PDFcell.NameDisplay = "Total Actual Time: ";
                return PDFcell;
            }

            if (prop.Name == ExternalProperties.TotalEstimatedTime.ToString())
            {
                PDFcell.NameDisplay = "Total Estimated Time: ";
                return PDFcell;
            }

            #endregion

            #region TotalFor headers 
             
            if (prop.Name == InternalProperties.TotalForActualTime.ToString())
            {
                PDFcell.NameDisplay = "Total For Actual Time: ";
                return PDFcell;
            }

            if (prop.Name == InternalProperties.TotalForEstimatedTime.ToString())
            {
                PDFcell.NameDisplay = "Total For Estimated Time: ";
                return PDFcell;
            }

            if (prop.Name == InternalProperties.ClientName.ToString() && value == Constants.WithoutClient.Name)
            {
                PDFcell.NameDisplay = string.Empty;
                return PDFcell;
            }

            if (prop.Name == InternalProperties.TimeEntryName.ToString())
            {
                PDFcell.NameDisplay = string.Empty;
                return PDFcell;
            }

            #endregion

            #region Entity headers

            if (prop.Name == InternalProperties.ProjectName.ToString())
            {
                PDFcell.NameDisplay = "Project";

                if (GroupById == (int)Constants.ReportsGroupBy.Project)
                {
                    PDFcell.NameDisplay = PDFcell.NameDisplay + ": ";
                }

                return PDFcell;
            }

            if (prop.Name == InternalProperties.ClientName.ToString())
            {
                PDFcell.NameDisplay = "Client";

                if (GroupById == (int)Constants.ReportsGroupBy.Client)
                {
                    PDFcell.NameDisplay = PDFcell.NameDisplay + ": ";
                }

                return PDFcell;
            }

            if (prop.Name == InternalProperties.Date.ToString())
            {
                PDFcell.NameDisplay = "Date";

                if (GroupById == (int) Constants.ReportsGroupBy.Date)
                {
                    PDFcell.NameDisplay = PDFcell.NameDisplay + ": ";
                }

                return PDFcell;
            }

            if (prop.Name.Contains(InternalProperties.MemberName.ToString()))
            {
                PDFcell.NameDisplay = "User";

                if (GroupById == (int)Constants.ReportsGroupBy.User)
                {
                    PDFcell.NameDisplay = PDFcell.NameDisplay + ": ";
                }

                return PDFcell;
            }

            if (prop.Name == InternalProperties.TaskName.ToString())
            {
                PDFcell.NameDisplay = "Task";
                return PDFcell;
            }

            if (prop.Name == InternalProperties.Description.ToString())
            {
                PDFcell.NameDisplay = "Notes";
                return PDFcell;
            }

            if (prop.Name == InternalProperties.ActualTime.ToString())
            {
                PDFcell.NameDisplay = "Actual Hours";
                return PDFcell;
            }

            if (prop.Name == InternalProperties.EstimatedTime.ToString())
            {
                PDFcell.NameDisplay = "Estimated Hours";
                return PDFcell;
            }

            if (prop.Name == InternalProperties.TimeFrom.ToString())
            {
                PDFcell.NameDisplay = "Start";
                return PDFcell;
            }

            if (prop.Name == InternalProperties.TimeTo.ToString())
            {
                PDFcell.NameDisplay = "Finish";
                return PDFcell;
            }

            #endregion

            return PDFcell;
        }

        #endregion

        #region Common Arrays of available Properties

        private List<PropertyInfo> PropsGroupByAndTotalTimes = new List<PropertyInfo>();
        private List<PropertyInfo> PropsEntityHeadersAndRows = new List<PropertyInfo>();
        private List<PropertyInfo> PropsEntitiesTotalHeaders = new List<PropertyInfo>();

        private string GetValueForCellPeriodDate()
        {
            var dateFormat = new GetDateFormat().GetDateFormaDotNetById(DateFormatId);
            var valueForPeriodCell = "Period: " + DateFrom.ToString(dateFormat, CultureInfo.InvariantCulture) + " - " + DateTo.ToString(dateFormat, CultureInfo.InvariantCulture);
            return valueForPeriodCell;
        }

        private string GetValueForTotaldCell()
        {
            return "TOTAL: ";
        }

        private ReportsCell GetPeriodPDFCell()
        {
            var dateFormat = new GetDateFormat().GetDateFormaDotNetById(DateFormatId);

            var pdfCell = new ReportsCell
            {
                NameDefault = "PeriodName",
                NameDisplay = "Period: ",
                Value = DateFrom.ToString(dateFormat) + " - " + DateTo.ToString(dateFormat),
            };

            return pdfCell;
        }

        #endregion

        private TCell CreateCell<T, TCell>(PropertyInfo prop, T entity) where TCell: ReportsCell, new()
        {
            var value = "EmptyValue";

            if (entity != null)
            {
                value = GetFormattedValueForCell(prop, entity);
            }

            var cell = GetNameDisplayForTotalHeaderPDF(prop, value);

            var grandEntityHeadersModel = new TCell
            {
                NameDefault = cell.NameDefault,
                NameDisplay = cell.NameDisplay,
                Value = value
            };

            return grandEntityHeadersModel;
        }

        private string GetFormattedValueForCell<T>(PropertyInfo prop, T entity)
        {
            var value = GetValueSingleFromProp(prop, entity);

            value = UpdateTimeFormatForValue(prop, value);
            value = UpdateProjectNameToUpperCase(prop, value);
            value = UpdateDateFormat(prop, value);
            value = ResetValueForGroupByNone(prop, value);

            return value;
        }

        #endregion
    }
}