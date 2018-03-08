using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Emails;
using MimeKit;
using System.Threading.Tasks;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        public async Task ExportEmailGroupByProjects(ReportsExportEmailView emailData)
        {
            var groupByProjects = _reportService.GetGroupingReportsGridByProjects(emailData);
            await SendReportWithGroupingAsync(emailData, groupByProjects);
        }

        public async Task ExportEmailGroupByUsers(ReportsExportEmailView emailData)
        {
            var groupByUsers = _reportService.GetGroupingReportsGridByMembers(emailData);
            await SendReportWithGroupingAsync(emailData, groupByUsers);
        }

        public async Task ExportEmailGroupByDates(ReportsExportEmailView emailData)
        {
            var groupByDates = _reportService.GetGroupingReportsGridByDates(emailData);
            await SendReportWithGroupingAsync(emailData, groupByDates);
        }

        public async Task ExportEmailGroupByClients(ReportsExportEmailView emailData)
        {
            var groupByClients = _reportService.GetGroupingReportsGridByClients(emailData);
            await SendReportWithGroupingAsync(emailData, groupByClients);
        }

        private async Task SendReportWithGroupingAsync<T>(ReportsExportEmailView emailData, IReportsTotalGridView<T> groupedList)
        {
            var fileByte = await CreateReportsFileOfBytesAsync(emailData, groupedList);

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