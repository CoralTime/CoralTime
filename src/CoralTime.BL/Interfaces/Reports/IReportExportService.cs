using CoralTime.ViewModels.Reports.Request.Emails;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoralTime.DAL.Models.Member;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportExportService
    {
        Task<FileResult> ExportFileReportsGridAsync(ReportsGridView reportsGridData, HttpContext httpContext);

        Task<object> ExportEmailGroupedByType(ReportsExportEmailView reportsExportEmailView, Member memberFromNotification = null, bool createMockTimeEntries = false);
    }
}