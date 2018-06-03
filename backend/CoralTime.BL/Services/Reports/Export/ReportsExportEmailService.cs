using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Emails;
using MimeKit;
using System.Threading.Tasks;
using CoralTime.DAL.Models;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        public async Task<object> ExportEmailGroupedByType(ReportsExportEmailView reportsExportEmailView, Member memberFromNotification = null)
        {
            var groupByProjects = _reportService.GetReportsGrid(reportsExportEmailView, memberFromNotification);
             
            await SendReportWithGroupingAsync(reportsExportEmailView, groupByProjects);

            return await Task.FromResult<object>(new object());
        }

        private async Task SendReportWithGroupingAsync(ReportsExportEmailView emailData, ReportTotalView groupedList)
        {
            var fileByte = await CreateFileOfBytesReportsGridAsync(emailData, groupedList);

            #region Create and send message with file Attachment.

            var builder = new BodyBuilder
            {
                TextBody = emailData.Comment ?? string.Empty,
            };

            // Add file to message body
            builder.Attachments.Add(FileName, fileByte);

            var mimeEntity = builder.ToMessageBody();

            var emailSender = new EmailSender(_configuration);
            emailSender.CreateSimpleMessage(emailData.ToEmail, mimeEntity, emailData.Subject ?? FileName, emailData.CcEmails, emailData.BccEmails);

            await emailSender.SendMessageAsync();

            #endregion
        }
    }
}