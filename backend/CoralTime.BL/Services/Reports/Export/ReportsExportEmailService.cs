using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Emails;
using MimeKit;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        public async Task<object> ExportEmailGroupedByType(ReportsExportEmailView reportsExportEmailView)
        {
            var groupByProjects = _reportService.GetReportsGroupingBy(reportsExportEmailView);
             
            //switch (reportsExportEmailView.CurrentQuery.GroupById)
            //{
            //    case (int) ReportsGroupBy.Project:
            //    {
            //        await ExportEmailGroupByProjects(reportsExportEmailView);
            //        break;
            //    }

            //    case (int) ReportsGroupBy.Member:
            //    {
            //        await ExportEmailGroupByUsers(reportsExportEmailView);
            //        break;
            //    }

            //    case (int) ReportsGroupBy.Date:
            //    {
            //        await ExportEmailGroupByDates(reportsExportEmailView);
            //        break;
            //    }

            //    case (int) ReportsGroupBy.Client:
            //    {
            //        await ExportEmailGroupByClients(reportsExportEmailView);
            //        break;
            //    }

            //    default:
            //    {
            //        return await Task.FromResult<object>(null);
            //    }
            //}

            await SendReportWithGroupingAsync(reportsExportEmailView, groupByProjects);

            return await Task.FromResult<object>(new object());
        }

        //private async Task ExportEmailGroupByProjects(ReportsExportEmailView reportsExportEmailView)
        //{
        //    var groupByProjects = _reportService.GetReportsGroupingBy(reportsExportEmailView);
        //    await SendReportWithGroupingAsync(reportsExportEmailView, groupByProjects);
        //}

        //private async Task ExportEmailGroupByUsers(ReportsExportEmailView emailData)
        //{
        //    var groupByUsers = _reportService.GetReportsGroupingBy(emailData);
        //    await SendReportWithGroupingAsync(emailData, groupByUsers);
        //}

        //private async Task ExportEmailGroupByDates(ReportsExportEmailView emailData)
        //{
        //    var groupByDates = _reportService.GetReportsGroupingBy(emailData);
        //    await SendReportWithGroupingAsync(emailData, groupByDates);
        //}

        //private async Task ExportEmailGroupByClients(ReportsExportEmailView emailData)
        //{
        //    var groupByClients = _reportService.GetReportsGroupingBy(emailData);
        //    await SendReportWithGroupingAsync(emailData, groupByClients);
        //}

        private async Task SendReportWithGroupingAsync(ReportsExportEmailView emailData, ReportTotalView groupedList)
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

            emailSender.CreateSimpleMessage(emailData.ToEmail, multipart, emailData.Subject ?? FileName,
                emailData.CcEmails, emailData.BccEmails);

            await emailSender.SendMessageAsync();

            #endregion
        }
    }
}