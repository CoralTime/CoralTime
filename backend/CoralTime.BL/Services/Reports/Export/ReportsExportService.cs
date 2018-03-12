using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.PDF;
using CoralTime.ViewModels.Reports.Request.Grid;
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
            TimeActual,
            TimeEstimated,
            Description,
            TotalForTimeActual,
            TotalForTimeEstimated,
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

        //public static readonly ShowColumnModel[] showColumnsInfo =
        //{
        //    new ShowColumnModel{ Id = (int) Constants.ShowColumnModelIds.ShowEstimatedTime, ShowColumnDescriptions = new List<ShowColumnDescription>
        //    {
        //        new ShowColumnDescription { Name = InternalProperties.TimeEstimated.ToString(), Description = "Show Estimated Hours" },
        //    }},
        //    new ShowColumnModel{ Id = (int) Constants.ShowColumnModelIds.ShowDate, ShowColumnDescriptions = new List<ShowColumnDescription>
        //    {
        //        new ShowColumnDescription { Name = InternalProperties.Date.ToString(), Description = "Show Date"}
        //    }},
        //    new ShowColumnModel{ Id = (int) Constants.ShowColumnModelIds.ShowNotes, ShowColumnDescriptions = new List<ShowColumnDescription>
        //    {
        //        new ShowColumnDescription { Name = InternalProperties.Description.ToString(), Description = "Show Notes"}
        //    }},
        //    new ShowColumnModel{ Id = (int) Constants.ShowColumnModelIds.ShowStartFinish, ShowColumnDescriptions = new List<ShowColumnDescription>
        //    {
        //        new ShowColumnDescription { Name = InternalProperties.TimeFrom.ToString(), Description = "Show Start Time"},
        //        new ShowColumnDescription { Name = InternalProperties.TimeTo.ToString(), Description = "Show Finish Time"}
        //    }}
        //};

        #endregion

        #region Export Excel, CSV, PDF. 

        public async Task<FileResult> ExportFileGroupedByTypeAsync(ReportsGridView reportsGridData, HttpContext httpContext)
        {
            var groupByType = _reportService.GetReportsGroupingBy(reportsGridData);

            var fileOfBytes = await CreateReportsFileOfBytesAsync(reportsGridData, groupByType);
            var fileStreamResult = SaveFileToFileStreamResult(httpContext, fileOfBytes);

            return fileStreamResult;
        }

        #endregion

        #region Export Excel, CSV, PDF. (Common methods)

        private async Task<byte[]> CreateReportsFileOfBytesAsync(ReportsGridView reportsGridView, ReportTotalView reportTotalView)
        {
            SetCommonValuesForExport(reportsGridView);

            var fileOfBytes = new byte[0];

            UpdateFileName();

            switch (reportsGridView.FileTypeId ?? 0)
            {
                case (int) FileType.Excel:
                {
                    FileName = FileName + ExtensionXLSX;
                    //fileOfBytes = CreateFileExcel(reportTotalView);
                    ContentType = ContentTypeXLSX;

                    break;
                }

                case (int) FileType.CSV:
                {
                    FileName = FileName + ExtensionCSV;
                    //file = CreateFileCSV(reportTotalView);
                    ContentType = ContentTypeCSV;

                    break;
                }

                case (int) FileType.PDF:
                {
                    FileName = FileName + ExtensionPDF;
                    fileOfBytes = await CreateFilePDFAsync(reportTotalView);
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

        private void SetCommonValuesForExport(ReportsGridView reportsGridData)
        {
            GroupById = SetGroupByOrDefaultGrouping(reportsGridData.CurrentQuery.GroupById);
            ShowColumnIds = reportsGridData.CurrentQuery.ShowColumnIds;

            DateFormatId = reportsGridData.DateFormatId;
            DateFrom = _reportService.DateFrom;
            DateTo = _reportService.DateTo;
        }

        private int SetGroupByOrDefaultGrouping(int? groupById)
        {
            return groupById ?? (int) Constants.ReportsGroupBy.Date;
        }

        #region Exclude excess properties

        /*
            1. Exlude one field ex:"ClientId" in external headers - ExcludeExternalPropInDependenceByGrouping() add field in Array "string[] _excludeProperties" from "enum InternalProperties".
            2. Exclude plural fields (Ids) in grid - ExcludeProps(): add field in Array "string[] _excludeProperties" from "enum ExcludePropertyByDefault"
            3. Exclude by custom options - ExcludeProps():           add field in Array "string[] _excludeProperties" from "enum ExternalProperties" AND "enum InternalProperties"
            4. Rename some properties.
        */

        //private bool IsGroupByThisProperty(string propName)
        //{
        //    var result = propName == InternalProperties.ProjectName.ToString() && GroupById == (int) Constants.ReportsGroupBy.Project
        //                 || propName == InternalProperties.MemberName.ToString() && GroupById == (int) Constants.ReportsGroupBy.Member
        //                 || propName == InternalProperties.Date.ToString() && GroupById == (int) Constants.ReportsGroupBy.Date
        //                 || propName == InternalProperties.ClientName.ToString() && GroupById == (int) Constants.ReportsGroupBy.Client
        //                 || propName == InternalProperties.TimeEntryName.ToString() && GroupById == (int) Constants.ReportsGroupBy.None;

        //    return result;
        //}

        //private bool IsPropTotalOrActualTime(string propName)
        //{
        //    var result = propName == InternalProperties.TotalForTimeActual.ToString() 
        //                 || propName == InternalProperties.TotalForTimeEstimated.ToString();
        //    return result;
        //}

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
            // TODO 
            //if (ShowColumnIds != null)
            //{
            //    foreach (var showColumnInfo in Constants.showColumnsInfo222)
            //    {
            //        var isAddPropToExcludeArray = !ShowColumnIds.Contains(showColumnInfo.Id);
            //        if (isAddPropToExcludeArray)
            //        {
            //            var showColumnDescriptionsById = Constants.showColumnsInfo222.FirstOrDefault(x => x.Id == showColumnInfo.Id)?.Description;
                        
            //            foreach (var tmpItem in showColumnDescriptionsById)
            //            {
            //                if (GroupById == (int)Constants.ReportsGroupBy.Date && showColumnDescriptionsById.Any(x => x.Name.Contains(InternalProperties.Date.ToString())))
            //                {
            //                    continue;
            //                }

            //                _excludeProperties = _excludeProperties.Append(tmpItem.Name).ToArray();
            //            }
            //        }
            //    }
            //}

            return _excludeProperties;
        }

        //private List<string> RenameNestedEntityHeaders(List<PropertyInfo> props)
        //{
        //    var result = new List<string>();

        //    foreach (var prop in props)
        //    {
        //        //if (IsGroupByThisProperty(prop.Name))
        //        //{
        //        //    continue;
        //        //}

        //        if (prop.Name == InternalProperties.ClientName.ToString())
        //        {
        //            result.Add("Client");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.MemberName.ToString())
        //        {
        //            result.Add("User");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.ProjectName.ToString())
        //        {
        //            result.Add("Project");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.TaskName.ToString())
        //        {
        //            result.Add("Task");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.Description.ToString())
        //        {
        //            result.Add("Notes");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.TimeActual.ToString())
        //        {
        //            result.Add("Act. Hours");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.TimeEstimated.ToString())
        //        {
        //            result.Add("Est. Hours");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.TimeFrom.ToString())
        //        {
        //            result.Add("Start");
        //            continue;
        //        }

        //        if (prop.Name == InternalProperties.TimeTo.ToString())
        //        {
        //            result.Add("Finish");
        //            continue;
        //        }

        //        result.Add(prop.Name);
        //    }

        //    return result;
        //}

        //public enum NameDisplays
        //{
        //    Notes
        //}

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

            if (propName == InternalProperties.TotalForTimeActual.ToString())
            {
                return "TOTAL FOR: ";
            }

            if (propName == InternalProperties.TotalForTimeEstimated.ToString())
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

                if (GroupById == (int)Constants.ReportsGroupBy.Member)
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

                if (GroupById == (int)Constants.ReportsGroupBy.Member)
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

        //private List<PropertyInfo> ExludePropByDefaultGrouping(List<PropertyInfo> props)
        //{
        //    var availableProps = new List<PropertyInfo>();

        //    foreach (var prop in props)
        //    {
        //        if (IsGroupByThisProperty(prop.Name))
        //        {
        //            continue;
        //        }

        //        availableProps.Add(prop);
        //    }

        //    return availableProps;
        //}

        private ReportsCell GetNameDisplayForTotalHeaderPDF(PropertyInfo prop, string value)
        {
            var PDFcell = new ReportsCell
            {
                NameDisplay = prop.Name,
                NameDefault = prop.Name
            };

            #region TotalFor headers 
             
            if (prop.Name == InternalProperties.ClientName.ToString() && value == Constants.WithoutClient.Name)
            {
                PDFcell.NameDisplay = string.Empty;
                return PDFcell;
            }

            //if (prop.Name == InternalProperties.TimeEntryName.ToString())
            //{
            //    PDFcell.NameDisplay = string.Empty;
            //    return PDFcell;
            //}

            #endregion

            return PDFcell;
        }

        #endregion

        #region Common Arrays of available Properties

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

        private string GetFormattedValueForCell<T>(PropertyInfo prop, T entity)
        {
            var value = GetValueSingleFromProp(prop, entity);

            value = UpdateTimeFormatForValue(prop, value);
            value = UpdateProjectNameToUpperCase(prop, value);
            value = UpdateDateFormat(prop, value);

            return value;
        }

        #endregion
    }
}