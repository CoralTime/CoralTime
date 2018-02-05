using CoralTime.ViewModels.Reports.Request.Emails;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportExportService
    {
        // Export Excel, CSV, PDF. Filtration By / Grouping By: None, Projects, Users, Dates, Clients.
        Task<FileResult> ExportFileGroupByNoneAsync(ReportsGridView reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByProjectsAsync(ReportsGridView reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByUsersAsync(ReportsGridView reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByDatesAsync(ReportsGridView reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByClientsAsync(ReportsGridView reportsGridData, HttpContext httpContext);

        // Sent reports as email.
        Task ExportEmailGroupByNone(ReportsExportEmailView emailData);

        Task ExportEmailGroupByProjects(ReportsExportEmailView emailData);

        Task ExportEmailGroupByUsers(ReportsExportEmailView emailData);

        Task ExportEmailGroupByDates(ReportsExportEmailView emailData);

        Task ExportEmailGroupByClients(ReportsExportEmailView emailData);
    }
}