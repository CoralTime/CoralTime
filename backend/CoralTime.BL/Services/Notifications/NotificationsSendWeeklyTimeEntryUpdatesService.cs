using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.ViewModels.Notifications;
using CoralTime.ViewModels.Reports.Request.Emails;
using CoralTime.ViewModels.Reports.Request.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoralTime.DAL.Models;

namespace CoralTime.BL.Services
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
                    CommonHelpers.SetRangeOfLastWorkWeekByDate(out var lastWorkWeekFirstDay, out var lastWorkWeekLastDay, todayDate);
                    var diffDates = (lastWorkWeekLastDay - lastWorkWeekFirstDay).TotalDays;
                    var editionPeriodDays = GetRangeNotificationDaysForLastWeek(lastWorkWeekFirstDay, diffDates);

                    var memberStartDayOfWeekStartByTodayDate = GetMemberStartDayOfWeekStartByTodayDate(todayDate);

                    var membersWithWeeklyTimeEntryUpdates = Uow.MemberRepository.LinkedCacheGetList()
                        .Where(member => member.IsWeeklyTimeEntryUpdatesSend &&member.WeekStart == memberStartDayOfWeekStartByTodayDate)
                        .Select(member => new
                        {
                            MemberId = member.Id,
                            MemberFullName = member.FullName,
                            MemberDateFormatId = member.DateFormatId,
                            MemberEmail = member.User.Email,

                            Projects = member.MemberProjectRoles.Where(mpr => mpr.Project.IsActive)
                            .Select(project => new
                            {
                                Id = project.Project.Id,
                                Name = project.Project.Name,
                            })
                        }).ToList();

                    foreach (var member in membersWithWeeklyTimeEntryUpdates.Where(x => x.MemberFullName == "YellowAdmin2_"))
                    {
                        var isNotFillTimeEntries = false;
                        var isAnyFillTimeEntries = false;

                        foreach (var project in member.Projects)
                        {
                            var dateTimeEntryByNotificationRange = Uow.TimeEntryRepository.GetQueryWithIncludes()
                                .Where(tEntry => tEntry.ProjectId == project.Id && tEntry.MemberId == member.MemberId)
                                .Where(tEntry => tEntry.Date.Date >= lastWorkWeekFirstDay && tEntry.Date.Date <= lastWorkWeekLastDay)
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
                            var memberWithProjectsNotifications = new MemberWithProjecsNotifications
                            {
                                MemberId = member.MemberId,
                                MemberFullName = member.MemberFullName,
                                MemberDateFormatId = member.MemberDateFormatId,
                                MemberEmail = member.MemberEmail
                            };

                            var subjectNotFilledTimeEnttries = CreateEmailSubjectWeeklyTimeEntryUpdates(memberWithProjectsNotifications.MemberEmail);

                            var emailText = CreateEmailTextWeeklyNotifications(baseUrl, memberWithProjectsNotifications.MemberFullName, isNotFillTimeEntries, isAnyFillTimeEntries);

                            var reportsExportEmailView = new ReportsExportEmailView
                            {
                                Comment = emailText,
                                DateFormatId = memberWithProjectsNotifications.MemberDateFormatId,
                                FileTypeId = (int) Constants.FileType.Excel,
                                Subject = subjectNotFilledTimeEnttries,
                                ToEmail = memberWithProjectsNotifications.MemberEmail,
                                CurrentQuery = new ReportsSettingsView
                                {
                                    DateFrom = lastWorkWeekFirstDay,
                                    DateTo = lastWorkWeekLastDay,
                                    GroupById = (int) Constants.ReportsGroupByIds.Project,
                                    ShowColumnIds = new[] { 1, 2, 3, 4 },
                                    ProjectIds = member.Projects.Select(x => x.Id).ToArray(),
                                    MemberIds = new[] { member.MemberId }
                                }
                            };

                            var memberFromNotification = Uow.MemberRepository.LinkedCacheGetById(member.MemberId);

                            await _reportExportService.ExportEmailGroupedByType(reportsExportEmailView, memberFromNotification, true);
                        }
                    }
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
