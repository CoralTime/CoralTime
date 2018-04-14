using CoralTime.ViewModels.Reports.Request.Emails;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportExportService
    {
        Task<FileResult> ExportFileReportsGridAsync(ReportsGridView reportsGridData, HttpContext httpContext);
        
        Task<object> ExportEmailGroupedByType(ReportsExportEmailView emailData);
    }
}