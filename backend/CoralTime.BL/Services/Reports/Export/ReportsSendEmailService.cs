using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Emails;
using MimeKit;
using System.Threading.Tasks;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        public async Task ExportEmailGroupByNone(string userName, ReportsExportEmailView emailData)
        {
            var groupByNone = _reportService.ReportsGridGroupByNone(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByNone);
        }

        public async Task ExportEmailGroupByProjects(string userName, ReportsExportEmailView emailData)
        {
            var groupByProjects = _reportService.ReportsGridGroupByProjects(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByProjects);
        }

        public async Task ExportEmailGroupByUsers(string userName, ReportsExportEmailView emailData)
        {
            var groupByUsers = _reportService.ReportsGridGroupByUsers(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByUsers);
        }

        public async Task ExportEmailGroupByDates(string userName, ReportsExportEmailView emailData)
        {
            var groupByDates = _reportService.ReportsGridGroupByDates(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByDates);
        }

        public async Task ExportEmailGroupByClients(string userName, ReportsExportEmailView emailData)
        {
            var groupByClients = _reportService.ReportsGridGroupByClients(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByClients);
        }

        private async Task SendReportWithGroupingAsync<T>(ReportsExportEmailView emailData, IReportsGrandGridView<T> groupedList)
        {
            var fileByte = CreateReportFileByteUpdateFileNameContentType(emailData, groupedList);

            #region Create and send message with file Attachment.

            // Create builder for message body 
            var builder = new BodyBuilder
            {
                TextBody = emailData.Comment ?? string.Empty,
            };

            // Add file to message body
            builder.Attachments.Add(FileName, fileByte);

            var multipart = builder.ToMessageBody();

            var emailSender = new EmailSender(_configuration);

            emailSender.CreateSimpleMessage(emailData.ToEmail, multipart, emailData.Subject ?? FileName, emailData.CcEmails, emailData.BccEmails);

            await emailSender.SendMessageAsync();

            #endregion
        }
    }
}