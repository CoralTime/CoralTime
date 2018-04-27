using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoralTime.BL.Services
{
    public class NotificationsService : BaseService, INotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationsService(UnitOfWork uow, IMapper mapper, IConfiguration configuration)
            : base(uow, mapper)
        {
            _configuration = configuration;
        }
        
        private class MemberWithProjecsNotifications
        {
            public int MemberId { get; set; }

            public string MemberFullName { get; set; }

            public int MemberDateFormatId { get; set; }

            public string MemberEmail { get; set; }

            public List<ProjectsWithDatesEditing> ProjectsWithDatesEditing { get; set; }

            public MemberWithProjecsNotifications()
            {
                ProjectsWithDatesEditing = new List<ProjectsWithDatesEditing>();
            }
        }

        private class ProjectsWithDatesEditing
        {
            public ProjectWithNotifications ProjectWithDatesEditing { get; set; }

            public ProjectEditionDays EditionDays { get; set; }

            public ProjectsWithDatesEditing()
            {
                ProjectWithDatesEditing = new ProjectWithNotifications();
                EditionDays = new ProjectEditionDays(); 
            }
        }

        private class ProjectEditionDays
        {
            public DateTime[] EditionDays { get; set; }

            public DateTime NotificationPeriodFirstDay { get; set; }

            public DateTime NotificationPeriodLastDay { get; set; }

            public ProjectEditionDays()
            {
                EditionDays = new DateTime[0];
            }
        }

        private class ProjectWithNotifications
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public int NotificationDay { get; set; }
        }

        public class EmailSenderSimpleModel
        {
            public string Subject { get; set; }

            public string ToEmail { get; set; }

            public string EmailText { get; set; }
        }

        public async Task EmailSenderSimple(IConfiguration configuration, EmailSenderSimpleModel emailSenderSimpleModel)
        {
            var body = new TextPart("html")
            {
                Text = emailSenderSimpleModel.EmailText
            };

            var emailSender = new EmailSender(configuration);

            emailSender.CreateSimpleMessage(emailSenderSimpleModel.ToEmail, new Multipart {body}, emailSenderSimpleModel.Subject);

            await emailSender.SendMessageAsync();
        }

        #region ByProjects Settings.

        public async Task ByProjectSettings(string baseUrl)
        {
            var todayDate = DateTime.Now;

            if (IsWorkDay(todayDate))
            {
                //CommonHelpers.SetRangeOfWorkWeekByDate(out var workWeekFirstDay, out var workWeekLastDay, todayDate);

                #region Active Private Projects with Notification Enable.

                var membersWithNotificationsProjects = GetMembersWithNotificationsProjects(todayDate);

                await SendNotificationsForMembers(membersWithNotificationsProjects, "Project", baseUrl);

                #endregion
            }
        }

        private List<MemberWithProjecsNotifications> GetMembersWithNotificationsProjects(DateTime todayDate)
        {
            var currentHour = todayDate.TimeOfDay.Hours;

            var membersWithNotifcationsProjects = new List<MemberWithProjecsNotifications>();

            var members = Uow.MemberRepository.LinkedCacheGetList()
                .Where(member => member.SendEmailTime == currentHour && member.MemberProjectRoles.Any(mpr => mpr.Project.IsNotificationEnabled && mpr.Project.IsActive && mpr.Project.IsPrivate && mpr.Project.NotificationDay > 0))
                .Select(member => new
                {
                    MemberId = member.Id,
                    MemberFullName = member.FullName,
                    MemberDateFormatId = member.DateFormatId,
                    MemberEmail = member.User.Email,

                    Projects = member.MemberProjectRoles.Where(mpr => mpr.Project.IsNotificationEnabled && mpr.Project.IsActive && mpr.Project.IsPrivate &&mpr.Project.NotificationDay > 0)
                        .Select(x => new
                        {
                            Id = x.Project.Id,
                            Name = x.Project.Name,
                            NotificationDay = x.Project.NotificationDay
                        }).ToList()
                }).ToList();

            foreach(var member in members)
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

                    var datesThatNotContainsTimeEntries= editionPeriodDays.Except(dateTimeEntryByNotificationRange).Select(g => g.Date.Date).ToArray();
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
                }

                membersWithNotifcationsProjects.Add(memberWithProjectsNotifications);
            }

            return membersWithNotifcationsProjects;
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

        #endregion

        #region Added methods

        private async Task SendNotificationsForMembers(List<MemberWithProjecsNotifications> membersWithProjecsNotifications, string subjectName, string baseUrl)
        {
            if (membersWithProjecsNotifications.Any())
            {
                foreach (var memberWithProjecsNotifications in membersWithProjecsNotifications)
                {
                    var sb = new StringBuilder($"<p>Hello, {memberWithProjecsNotifications.MemberFullName}!<br><p>This is a friendly reminder, that you haven’t entered your Time Entries on ");

                    if (memberWithProjecsNotifications.ProjectsWithDatesEditing.Count > 1)
                    {
                        sb.Append("the following projects:<br>");
                    }

                    var dateFormatShort = new GetDateFormat().GetDateFormaDotNetShortById(memberWithProjecsNotifications.MemberDateFormatId);

                    var indexCurrentProject = 0;
                    foreach (var project in memberWithProjecsNotifications.ProjectsWithDatesEditing)
                    {
                        var dayOrDays = project.EditionDays.EditionDays.Length > 1 ? "days" : "day";

                        if (memberWithProjecsNotifications.ProjectsWithDatesEditing.Count > 1)
                        {
                            sb.Append($"{++indexCurrentProject}. ");
                        }

                        var editionDaysFormat = string.Join(", </b><b>", project.EditionDays.EditionDays.Select(x => x.ToString(dateFormatShort, CultureInfo.InvariantCulture)));
                        var sbEditionDaysFormat = new StringBuilder($"<b>{editionDaysFormat}</b>");

                        sb.Append($"<b>{project.ProjectWithDatesEditing.Name}</b> project for <b>{project.EditionDays.EditionDays.Length}</b> work{dayOrDays}: {sbEditionDaysFormat}.<br>");
                    }

                    sb.Append($"<p><a href=\"{baseUrl}/calendar/\">Would you like to enter your time now?</a><br><p>Best wishes, <a href=\"mailto:coraltime2017@yandex.ru\">CoralTime Team!</a>");

                    var emailSenderSimpleModel = new EmailSenderSimpleModel
                    {
                        Subject = $"Reminder to fill time entry {memberWithProjecsNotifications.MemberEmail} by {subjectName} Settigs",
                        ToEmail = memberWithProjecsNotifications.MemberEmail,
                        EmailText = sb.ToString()
                    };

                    await EmailSenderSimple(_configuration, emailSenderSimpleModel);
                }
            }
        }

        #endregion
    }
} 
