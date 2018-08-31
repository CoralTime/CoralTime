using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Request.MemberWithProjectsLightIds;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Responce;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Responce.MemberWithProjectsLight;

namespace CoralTime.BL.Services.Notifications
{
    public partial class NotificationsService
    {
        public async Task ByProjectSettingsAsync(string baseUrl)
        {
            var todayDate = DateTime.Now;

            if (IsWorkDay(todayDate))
            {
                await SendToMemberNotificationsByProjectsSettingsAsync(todayDate, baseUrl);
            }
        }

        private async Task SendToMemberNotificationsByProjectsSettingsAsync(DateTime todayDate, string baseUrl, List<MemberWithProjectsIdsView> memberWithProjectsIds = null)
        {
            var members = GetMembersWithProjectsNotification(memberWithProjectsIds);

            foreach (var member in members)
            {
                var memberWithProjectsNotificationsForEmail = new MemberWithProjecsNotificationsView
                {
                    MemberLight = new MemberLightView
                    {
                        Id = member.Id,
                        FullName = member.FullName,
                        DateFormatId = member.DateFormatId,
                        Email = member.Email
                    }
                };

                var emailTextByProjectSettings = string.Empty;
                var subjectByProjectSettings = string.Empty;

                foreach (var project in member.Projects)
                {
                    var editionPeriodDays = GetRangeNotificationDays(todayDate, project.NotificationDay, out var notificationPeriodFirstDay, out var notificationPeriodLastDay);

                    var dateTimeEntryByNotificationRange = Uow.TimeEntryRepository.GetQuery(withIncludes: false, asNoTracking: true)
                        .Where(tEntry => tEntry.ProjectId == project.Id && tEntry.MemberId == member.Id)
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
                            },

                            EditionDays = new ProjectEditionDays
                            {
                                EditionDays = datesThatNotContainsTimeEntries,
                                NotificationPeriodFirstDay = notificationPeriodFirstDay,
                                NotificationPeriodLastDay = notificationPeriodLastDay
                            }
                        };

                        memberWithProjectsNotificationsForEmail.ProjectsWithDatesEditing.Add(projectWithDatesEditing);
                    }

                    if (memberWithProjectsNotificationsForEmail.ProjectsWithDatesEditing.Count == 0) 
                        continue;
                    
                    subjectByProjectSettings = CreateEmailSubjectByProjectSettings();
                    emailTextByProjectSettings = CreateEmailTextForEmailByProjectSettings(baseUrl, memberWithProjectsNotificationsForEmail);
                }

                if (emailTextByProjectSettings != string.Empty)
                {
                    await CreateAndSendEmailNotificationForUserAsync(emailTextByProjectSettings, memberWithProjectsNotificationsForEmail.MemberLight.Email, subjectByProjectSettings);
                }
            }
        }

        public List<MemberWithProjectsLightView> GetMembersWithProjectsNotification(List<MemberWithProjectsIdsView> memberWithProjectsIds = null)
        {
            var mprByMemberIdAndProjectIds = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(mpr => memberWithProjectsIds?.Select(memberId => memberId.MemberId).Contains(mpr.MemberId) ?? true)
                .Where(mpr => memberWithProjectsIds?.Select(memberId => memberId.ProjectIds).Any(projectIds => projectIds.Contains(mpr.ProjectId)) ?? true)
                .Where(mpr => mpr.Project.IsNotificationEnabled && mpr.Project.IsActive && mpr.Project.IsPrivate && mpr.Project.NotificationDay > 0);

            var result = mprByMemberIdAndProjectIds.GroupBy(mpr => mpr.Member)
                .Select(items => new MemberWithProjectsLightView
                {
                    Id = items.Key.Id,
                    FullName = items.Key.FullName,
                    DateFormatId = items.Key.DateFormatId,
                    Email = items.Key.User.Email,

                    Projects = items.Select(mpr => new ProjectLightView
                    {
                        Id = mpr.Project.Id,
                        Name = mpr.Project.Name,
                        NotificationDay = mpr.Project.NotificationDay
                    }).ToList()
                }).ToList();

            return result;
        }

        private static bool IsWorkDay(DateTime date) => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

        private static DateTime[] GetRangeNotificationDays(DateTime todayDate, int projectNotificationDayCount, out DateTime notificationPeriodFirstDay, out DateTime notificationPeriodLastDay)
        {
            if (projectNotificationDayCount <= 0)
            {
                throw new CoralTimeDangerException("You cannot invoke this function that project.NotificationDayCount <= 0 days");
            }

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

        private static string CreateEmailSubjectByProjectSettings() => $"Reminder to fill Time Entries";

        private static string CreateEmailTextForEmailByProjectSettings(string baseUrl, MemberWithProjecsNotificationsView memberWithProjecsNotifications)
        {
            var sbEmailText = new StringBuilder($"<p>Hello, {memberWithProjecsNotifications.MemberLight.FullName}!<br><p>This is a friendly reminder, that you haven’t entered your Time Entries on ");

            if (memberWithProjecsNotifications.ProjectsWithDatesEditing.Count > 1)
            {
                sbEmailText.Append("the following projects:<br>");
            }

            var dateFormatShort = DateFormatsStorage.GetDateFormatDotNetShortById(memberWithProjecsNotifications.MemberLight.DateFormatId);

            var indexCurrentProject = 0;
            foreach (var project in memberWithProjecsNotifications.ProjectsWithDatesEditing)
            {
                var dayOrDays = project.EditionDays.EditionDays.Length > 1 ? "days" : "day";

                if (memberWithProjecsNotifications.ProjectsWithDatesEditing.Count > 1)
                {
                    sbEmailText.Append($"{++indexCurrentProject}. ");
                }

                var sbEditionDaysFormat = new StringBuilder($"<b>{string.Join(", </b><b>", project.EditionDays.EditionDays.Select(x => x.ToString(dateFormatShort, CultureInfo.InvariantCulture)))}</b>");

                sbEmailText.Append($"<b>{project.ProjectWithDatesEditing.Name}</b> project for <b>{project.EditionDays.EditionDays.Length}</b> work{dayOrDays}: {sbEditionDaysFormat}.<br>");
            }

            sbEmailText.Append($"<p><a href=\"{baseUrl}/calendar/\">Would you like to enter your time now?</a><br><p>Best wishes, <a href=\"mailto:coraltime2017@yandex.ru\">CoralTime Team!</a>");

            return sbEmailText.ToString();
        }
    }
}
