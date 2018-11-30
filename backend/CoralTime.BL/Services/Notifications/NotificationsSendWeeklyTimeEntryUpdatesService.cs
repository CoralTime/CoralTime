using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Responce;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Responce.MemberWithProjectsLight;
using CoralTime.ViewModels.Reports.Request.Emails;
using CoralTime.ViewModels.Reports.Request.ReportsSettingsView;

namespace CoralTime.BL.Services.Notifications
{
    public partial class NotificationsService
    {
        private string CreateEmailSubjectWeeklyTimeEntryUpdates(string emailMember) => new StringBuilder($"Reminder to fill Time Entries {emailMember} by Weekly TimeEntry Updates").ToString();

        public async Task SendWeeklyTimeEntryUpdatesAsync(string baseUrl)
        {
            var todayDate = DateTime.Now;

            if (IsDayOfWeekStart(todayDate))
            {
                var currentHour = todayDate.TimeOfDay.Hours;

                if (currentHour == 1)
                {
                    await SendWeeklyNotificationsForMembers(baseUrl);
                }
            }
        }

        private async Task SendWeeklyNotificationsForMembers(string baseUrl, int [] membersIds = null)
        {
            var lastworkWeek = CommonHelpers.GetRangeOfLastWorkWeekByDate(DateTime.Today);
            var diffDates = (lastworkWeek.DateTo - lastworkWeek.DateFrom).TotalDays;
            var editionPeriodDays = GetRangeNotificationDaysForLastWeek(lastworkWeek.DateTo, diffDates);

            var memberStartDayOfWeekStartByTodayDate = GetMemberStartDayOfWeekStartByTodayDate(DateTime.Today);

            var membersWithWeeklyTimeEntryUpdates = Uow.MemberRepository.LinkedCacheGetList()
                .Where(member => membersIds?.Contains(member.Id) ?? true)
                .Where(member => member.IsWeeklyTimeEntryUpdatesSend && member.WeekStart == memberStartDayOfWeekStartByTodayDate)
                .Select(member => new
                {
                    MemberId = member.Id,
                    MemberFullName = member.FullName,
                    MemberDateFormatId = member.DateFormatId,
                    MemberEmail = member.User.Email,

                    Projects = member.MemberProjectRoles.Where(mpr => mpr.Project.IsActive)
                        .Select(project => new
                        {
                            project.Project.Id,
                            project.Project.Name,
                        })
                }).ToList();

            foreach (var member in membersWithWeeklyTimeEntryUpdates)
            {
                var isNotFillTimeEntries = false;
                var isAnyFillTimeEntries = false;

                foreach (var project in member.Projects)
                {
                    var dateTimeEntryByNotificationRange = Uow.TimeEntryRepository.GetQuery()
                        .Where(tEntry => tEntry.ProjectId == project.Id && tEntry.MemberId == member.MemberId)
                        .Where(tEntry => tEntry.Date.Date >= lastworkWeek.DateTo && tEntry.Date.Date <= lastworkWeek.DateFrom)
                        .Select(tEntry => tEntry.Date.Date)
                        .ToList();

                    var datesWithoutTimeEntries = editionPeriodDays.Except(dateTimeEntryByNotificationRange).ToArray();

                    var hasNotTimeEntries = dateTimeEntryByNotificationRange.Count == 0;
                    var hasAnyTimeEntries = dateTimeEntryByNotificationRange.Any() && datesWithoutTimeEntries.Any();
                    //var hasAllTimeEntries = datesWithoutTimeEntries.Length == 0;

                    if (hasNotTimeEntries)
                    {
                        if (!isAnyFillTimeEntries)
                        {
                            isNotFillTimeEntries = true;
                        }
                    }

                    if (hasAnyTimeEntries)
                    {
                        isNotFillTimeEntries = false;
                        isAnyFillTimeEntries = true;
                    }
                }

                if (isNotFillTimeEntries || isAnyFillTimeEntries)
                {
                    var memberWithProjectsNotifications = new MemberWithProjecsNotificationsView
                    {
                        MemberLight = new MemberLightView
                        {
                            Id = member.MemberId,
                            FullName = member.MemberFullName,
                            DateFormatId = member.MemberDateFormatId,
                            Email = member.MemberEmail,
                        }
                    };

                    var subjectNotFilledTimeEnttries = CreateEmailSubjectWeeklyTimeEntryUpdates(memberWithProjectsNotifications.MemberLight.Email);

                    var emailText = CreateEmailTextWeeklyNotifications(baseUrl, memberWithProjectsNotifications.MemberLight.FullName, isNotFillTimeEntries, isAnyFillTimeEntries);

                    var reportsExportEmailView = new ReportsExportEmailView
                    {
                        Comment = emailText,
                        DateFormatId = memberWithProjectsNotifications.MemberLight.DateFormatId,
                        FileTypeId = (int) Constants.FileType.Excel,
                        Subject = subjectNotFilledTimeEnttries,
                        ToEmail = memberWithProjectsNotifications.MemberLight.Email,
                        CurrentQuery = new ReportsSettingsView
                        {
                            DateFrom = lastworkWeek.DateTo,
                            DateTo = lastworkWeek.DateFrom,
                            GroupById = (int) Constants.ReportsGroupByIds.Project,
                            ShowColumnIds = new[] {1, 2, 3, 4},
                            ProjectIds = member.Projects.Select(x => x.Id).ToArray(),
                            MemberIds = new[] {member.MemberId}
                        }
                    };

                    var memberFromNotification = Uow.MemberRepository.LinkedCacheGetById(member.MemberId);

                    await _reportExportService.ExportEmailGroupedByType(reportsExportEmailView, memberFromNotification, true);
                }
            }
        }

        private string CreateEmailTextWeeklyNotifications(string baseUrl, string memberFullName, bool isNotFillTimeEntries, bool isAnyFillTimeEntries)
        {
            var sbEmailText = new StringBuilder($"Hello, {memberFullName}! ");

            if (isNotFillTimeEntries)
            {
                sbEmailText.Append("It seems you haven’t filled any Time Entries for the last week. ");
                sbEmailText.Append("Would you like to track your time now?");
            }

            if (isAnyFillTimeEntries)
            {
                sbEmailText.Append("You could find your hours tracked last week attached to this letter. ");
                sbEmailText.Append("Would you like to change your time entries now?");
            }

            //sbEmailText.Append($"<a href=\"{baseUrl}/calendar/\">");
            sbEmailText.Append($"{baseUrl}/calendar/\"");

            return sbEmailText.ToString();
        }

        private bool IsDayOfWeekStart(DateTime date) => date.DayOfWeek == DayOfWeek.Monday || date.DayOfWeek == DayOfWeek.Sunday;

        private Constants.WeekStart GetMemberStartDayOfWeekStartByTodayDate(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                {
                    return Constants.WeekStart.Monday;
                }

                case DayOfWeek.Sunday:
                {
                    return Constants.WeekStart.Sunday;
                }
               
                default:
                {
                    throw new CoralTimeAlreadyExistsException("Member has incorrect day of week start");
                }
            }
        }

        private DateTime[] GetRangeNotificationDaysForLastWeek(DateTime notificationPeriodFirstDay, double diffDates)
        {
            var notificationPeriodDays = new List<DateTime>();

            for (var i = 0; i < diffDates; i++)
            {
                notificationPeriodDays.Add(notificationPeriodFirstDay.Date.AddDays(i));
            }

            return notificationPeriodDays.ToArray();
        }
    }
}
