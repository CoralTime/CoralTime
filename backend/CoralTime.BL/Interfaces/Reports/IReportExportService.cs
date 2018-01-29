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
        Task<FileResult> ExportFileGroupByNoneAsync(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByProjectsAsync(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByUsersAsync(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByDatesAsync(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        Task<FileResult> ExportFileGroupByClientsAsync(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        // Sent reports as email.
        Task ExportEmailGroupByNone(string userName, ReportsExportEmailView emailData);

        Task ExportEmailGroupByProjects(string userName, ReportsExportEmailView emailData);

        Task ExportEmailGroupByUsers(string userName, ReportsExportEmailView emailData);

        Task ExportEmailGroupByDates(string userName, ReportsExportEmailView emailData);

        Task ExportEmailGroupByClients(string userName, ReportsExportEmailView emailData);
    }
}