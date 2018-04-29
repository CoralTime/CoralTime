using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Notifications;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoralTime.BL.Services
{
    public partial class NotificationsService
    {
        #region ByProjectsSettings.

        public async Task ByProjectSettingsAsync(string baseUrl)
        {
            var todayDate = DateTime.Now;

            if (IsWorkDay(todayDate))
            {
                //CommonHelpers.SetRangeOfWorkWeekByDate(out var workWeekFirstDay, out var workWeekLastDay, todayDate);

                await GetMembersWithNotificationsProjectsAsync(todayDate, baseUrl);
            }
        }

        private async Task GetMembersWithNotificationsProjectsAsync(DateTime todayDate, string baseUrl)
        {
            var currentHour = todayDate.TimeOfDay.Hours;

            var members = Uow.MemberRepository.LinkedCacheGetList()
                .Where(member => member.SendEmailTime == currentHour && member.MemberProjectRoles.Any(mpr => mpr.Project.IsNotificationEnabled && mpr.Project.IsActive && mpr.Project.IsPrivate && mpr.Project.NotificationDay > 0))
                .Select(member => new
                {
                    MemberId = member.Id,
                    MemberFullName = member.FullName,
                    MemberDateFormatId = member.DateFormatId,
                    MemberEmail = member.User.Email,

                    Projects = member.MemberProjectRoles.Where(mpr => mpr.Project.IsNotificationEnabled && mpr.Project.IsActive && mpr.Project.IsPrivate && mpr.Project.NotificationDay > 0)
                        .Select(x => new
                        {
                            Id = x.Project.Id,
                            Name = x.Project.Name,
                            NotificationDay = x.Project.NotificationDay
                        }).ToList()
                }).ToList();

            foreach (var member in members)
            {
                var memberWithProjectsNotifications = new MemberWithProjecsNotifications();

                memberWithProjectsNotifications.MemberId = member.MemberId;
                memberWithProjectsNotifications.MemberFullName = member.MemberFullName;
                memberWithProjectsNotifications.MemberDateFormatId = member.MemberDateFormatId;
                memberWithProjectsNotifications.MemberEmail = member.MemberEmail;

                foreach (var project in member.Projects)
                {
                    var editionPeriodDays = GetNotificationPeriodDays(todayDate, project.NotificationDay, out var notificationPeriodFirstDay, out var notificationPeriodLastDay);

                    var dateTimeEntryByNotificationRange = Uow.TimeEntryRepository.GetQueryWithIncludes()
                        .Where(tEntry => tEntry.ProjectId == project.Id && tEntry.MemberId == member.MemberId)
                        .Where(tEntry => tEntry.Date.Date >= notificationPeriodFirstDay && tEntry.Date.Date <= notificationPeriodLastDay)
                        .Select(tEntry => tEntry.Date)
                        .ToList();

                    var datesThatNotContainsTimeEntries = editionPeriodDays.Except(dateTimeEntryByNotificationRange).Select(g => g.Date.Date).ToArray();
                    if (datesThatNotContainsTimeEntries.Length > 0)
                    {
                        var projectWithDatesEditing = new ProjectsWithDatesEditing
                        {
                            ProjectWithDatesEditing = new ProjectWithNotifications
                            {
                                Id = project.Id,
                                Name = project.Name,
                                NotificationDay = project.NotificationDay
                            },

                            EditionDays = new ProjectEditionDays
                            {
                                EditionDays = datesThatNotContainsTimeEntries,
                                NotificationPeriodFirstDay = notificationPeriodFirstDay,
                                NotificationPeriodLastDay = notificationPeriodLastDay
                            }
                        };

                        memberWithProjectsNotifications.ProjectsWithDatesEditing.Add(projectWithDatesEditing);
                    }

                    await CreateAndSendEmailNotificationForUserAsync("Project", baseUrl, memberWithProjectsNotifications);
                }
            }
        }

        private bool IsWorkDay(DateTime date) => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

        private DateTime[] GetNotificationPeriodDays(DateTime todayDate, int projectNotificationDayCount, out DateTime notificationPeriodFirstDay, out DateTime notificationPeriodLastDay)
        {
            var notificationPeriodDays = new List<DateTime>();

            var notificationPeriodDay = todayDate.Date.AddDays(-1);

            do
            {
                if (IsWorkDay(notificationPeriodDay))
                {
                    notificationPeriodDays.Add(notificationPeriodDay);
                    --projectNotificationDayCount;
                }

                notificationPeriodDay = notificationPeriodDay.AddDays(-1);
            } while (projectNotificationDayCount > 0);

            notificationPeriodFirstDay = notificationPeriodDays.Min(x => x.Date);
            notificationPeriodLastDay = notificationPeriodDays.Max(x => x.Date);

            return notificationPeriodDays.OrderBy(x => x.Date).ToArray();
        }

        //private DateTime GetLastDayEditionRange(DateTime todayDate)
        //{
        //    var lastDayEditionRange = todayDate.Date.AddMilliseconds(-1);
        //    do
        //    {
        //        lastDayEditionRange = lastDayEditionRange.AddDays(-1);
        //    } while (!IsWorkDay(lastDayEditionRange));

        //    return lastDayEditionRange;
        //}

        private async Task CreateAndSendEmailNotificationForUserAsync(string subjectName, string baseUrl, MemberWithProjecsNotifications memberWithProjecsNotifications)
        {
            var sbEmailText = new StringBuilder($"<p>Hello, {memberWithProjecsNotifications.MemberFullName}!<br><p>This is a friendly reminder, that you haven’t entered your Time Entries on ");

            if (memberWithProjecsNotifications.ProjectsWithDatesEditing.Count > 1)
            {
                sbEmailText.Append("the following projects:<br>");
            }

            var dateFormatShort = new GetDateFormat().GetDateFormaDotNetShortById(memberWithProjecsNotifications.MemberDateFormatId);

            var indexCurrentProject = 0;
            foreach (var project in memberWithProjecsNotifications.ProjectsWithDatesEditing)
            {
                var dayOrDays = project.EditionDays.EditionDays.Length > 1 ? "days" : "day";

                if (memberWithProjecsNotifications.ProjectsWithDatesEditing.Count > 1)
                {
                    sbEmailText.Append($"{++indexCurrentProject}. ");
                }

                var editionDaysFormat = string.Join(", </b><b>", project.EditionDays.EditionDays.Select(x => x.ToString(dateFormatShort, CultureInfo.InvariantCulture)));
                var sbEditionDaysFormat = new StringBuilder($"<b>{editionDaysFormat}</b>");

                sbEmailText.Append($"<b>{project.ProjectWithDatesEditing.Name}</b> project for <b>{project.EditionDays.EditionDays.Length}</b> work{dayOrDays}: {sbEditionDaysFormat}.<br>");
            }

            sbEmailText.Append($"<p><a href=\"{baseUrl}/calendar/\">Would you like to enter your time now?</a><br><p>Best wishes, <a href=\"mailto:coraltime2017@yandex.ru\">CoralTime Team!</a>");

            var emailSenderSimpleModel = new EmailSenderSimpleModel
            {
                Subject = $"Reminder to fill time entry {memberWithProjecsNotifications.MemberEmail} by {subjectName} Settigs",
                ToEmail = memberWithProjecsNotifications.MemberEmail,
                EmailText = sbEmailText.ToString()
            };

            await EmailSenderSimple(_configuration, emailSenderSimpleModel);
        }

        #endregion
    }
}
