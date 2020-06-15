using System.Collections.Generic;
using System.Linq;
using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Reports.Request.Emails;
using MimeKit;
using System.Threading.Tasks;
using CoralTime.Common.Constants;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Models;
using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal;

namespace CoralTime.BL.Services.Reports.Export
{
    public partial class ReportsExportService
    {
        public async Task<object> ExportEmailGroupedByType(ReportsExportEmailView reportsExportEmailView, Member memberFromNotification = null, bool createMockTimeEntries = false)
        {
            var groupByProjects = _reportService.GetReportsGrid(reportsExportEmailView, memberFromNotification);

            if (createMockTimeEntries)
            {
                if (groupByProjects.GroupedItems.Count == 0)
                {
                    groupByProjects = CreateMockTimeEntriesGroupByProjects(reportsExportEmailView, memberFromNotification);
                }
            }

            await SendReportWithGroupingAsync(reportsExportEmailView, groupByProjects);

            return await Task.FromResult<object>(new object());
        }

        #region CreateMockTimeEntriesGroupByProjects

        private ReportTotalView CreateMockTimeEntriesGroupByProjects(ReportsExportEmailView reportsExportEmailView, Member memberFromNotification)
        {
            var reportTotalView = _reportService.InitializeReportTotalView(reportsExportEmailView);

            var projects = Uow.ProjectRepository.LinkedCacheGetList().Where(project => reportsExportEmailView.CurrentQuery.ProjectIds.Contains(project.Id));

            var emptyTimeEntries = new List<TimeEntry>();

            foreach (var project in projects)
            {
                emptyTimeEntries.Add(CreateMockTimeEntry(memberFromNotification, project));
            }

            var timeEntriesGroupByProjects = emptyTimeEntries
                .GroupBy(i => i.Project)
                .OrderBy(x => x.Key.Name)
                .ToDictionary(key => key.Key, value => value.OrderBy(x => x.Date).ToList());

            reportTotalView.GetView(timeEntriesGroupByProjects);

            return reportTotalView;
        }

        private TimeEntry CreateMockTimeEntry(Member memberFromNotification, Project project) => new TimeEntry
        {
            Description = "This is Mock TimeEntry",
            TimeActual = Constants.MockId,
            Project = project,
            ProjectId = project.Id,
            MemberId = memberFromNotification.Id,
            Member = memberFromNotification,
            TaskType = new TaskType {Id = Constants.MockId, Name = "This is Mock Task"},
            TaskTypesId = Constants.MockId
        };

        #endregion

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