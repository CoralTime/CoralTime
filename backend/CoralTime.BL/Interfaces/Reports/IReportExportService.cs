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
        Task<FileResult> ExportFileGroupByNoneAsync(RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByProjectsAsync(RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByUsersAsync(RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByDatesAsync(RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByClientsAsync(RequestReportsGrid reportsGridData, HttpContext httpContext);

        // Sent reports as email.
        Task ExportEmailGroupByNone(ReportsExportEmailView emailData);

        Task ExportEmailGroupByProjects(ReportsExportEmailView emailData);

        Task ExportEmailGroupByUsers(ReportsExportEmailView emailData);

        Task ExportEmailGroupByDates(ReportsExportEmailView emailData);

        Task ExportEmailGroupByClients(ReportsExportEmailView emailData);
    }
}