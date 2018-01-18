using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Emails;
using MimeKit;
using System.Threading.Tasks;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportExportService
    {
        public async Task SentGroupByNone(string userName, ReportsSendAsEmailView emailData)
        {
            var groupByNone = _reportService.ReportsGridGroupByNone(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByNone);
        }

        public async Task SentGroupByProjects(string userName, ReportsSendAsEmailView emailData)
        {
            var groupByProjects = _reportService.ReportsGridGroupByProjects(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByProjects);
        }

        public async Task SentGroupByUsers(string userName, ReportsSendAsEmailView emailData)
        {
            var groupByUsers = _reportService.ReportsGridGroupByUsers(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByUsers);
        }

        public async Task SentGroupByDates(string userName, ReportsSendAsEmailView emailData)
        {
            var groupByDates = _reportService.ReportsGridGroupByDates(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByDates);
        }

        public async Task SentGroupByClients(string userName, ReportsSendAsEmailView emailData)
        {
            var groupByClients = _reportService.ReportsGridGroupByClients(userName, emailData);
            await SendReportWithGroupingAsync(emailData, groupByClients);
        }

        private async Task SendReportWithGroupingAsync<T>(ReportsSendAsEmailView emailData, IReportsGrandGridView<T> groupedList)
        {
            var fileByte = CreateReportFileByteUpdateFileNameContentType(emailData, groupedList, out var contentType);

            #region Create and send message with file Attachment.

            // Create builder for message body 
            var builder = new BodyBuilder
            {
                TextBody = emailData.Comment ?? string.Empty,
            };

            // Add file to message body
            builder.Attachments.Add(fileName, fileByte);

            var multipart = builder.ToMessageBody();

            var emailSender = new EmailSender(_configuration);

            emailSender.CreateSimpleMessage(emailData.ToEmail, multipart, emailData.Subject ?? fileName, emailData.CcEmails, emailData.BccEmails);

            await emailSender.SendMessageAsync();

            #endregion
        }
    }
}