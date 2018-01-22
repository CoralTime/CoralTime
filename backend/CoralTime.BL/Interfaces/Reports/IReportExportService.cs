using CoralTime.Common.Models.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Request.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportExportService
    {
        // Export Excel, CSV, PDF. Filtration By / Grouping By: None, Projects, Users, Dates, Clients.
        FileResult ExportFileGroupByNone(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        FileResult ExportFileGroupByProjects(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        FileResult ExportFileGroupByUsers(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        FileResult ExportFileGroupByDates(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        FileResult ExportFileGroupByClients(string userName, RequestReportsGrid reportsGridData, HttpContext httpContext);

        // Sent reports as email.
        Task ExportEmailGroupByNone(string userName, ReportsExportSendView emailData);

        Task ExportEmailGroupByProjects(string userName, ReportsExportSendView emailData);

        Task ExportEmailGroupByUsers(string userName, ReportsExportSendView emailData);

        Task ExportEmailGroupByDates(string userName, ReportsExportSendView emailData);

        Task ExportEmailGroupByClients(string userName, ReportsExportSendView emailData);
    }
}