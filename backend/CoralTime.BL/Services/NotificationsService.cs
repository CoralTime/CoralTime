using AutoMapper;
using CoralTime.BL.ServicesInterfaces;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoralTime.BL.Services
{
    public class NotificationsService: _BaseService, INotificationService
    {
        private readonly IConfiguration _configuration;
        
        public NotificationsService(UnitOfWork uow, IMapper mapper, IConfiguration configuration)
            :base(uow, mapper)
        {
            _configuration = configuration;
        }

        private DateTime NotificationPeriodFirstDay { get; set; }

        private DateTime NotificationPeriodLastDay { get; set; }

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

            emailSender.CreateSimpleMessage(emailSenderSimpleModel.ToEmail, new Multipart { body }, emailSenderSimpleModel.Subject);

            await emailSender.SendMessageAsync();
        }

        #region ByMemberSettings

        public async Task ByMemberSettings()
        {
            var todayDate = DateTime.Now;
            var currentHour = todayDate.TimeOfDay.Hours;
            var currentDayOfWeek = todayDate.DayOfWeek;

            CommonHelpers.SetRangeOfWorkWeekByDate(out var workWeekFirstDay, out var workWeekLastDay, todayDate);

            var projectsWithNotificationEnable = GetPrivateProjectsWithNotificationEnable();
            if (projectsWithNotificationEnable.Any())
            {
                foreach (var project in projectsWithNotificationEnable)
                {
                    var membersWithoutTimeEntry = GetMembersWithoutTimeEntryForPrivateProjects(project, todayDate);

                    // Get members who choose in options current DayOfWeek and Hour.
                    var membersByDayOfWeekAndHour = GetMembersByDayOfWeekAndHour(membersWithoutTimeEntry, currentDayOfWeek, currentHour);

                    await SendNotificationsForMembers(membersByDayOfWeekAndHour, project, "Member");
                }
            }
        }

        private List<Member> GetMembersByDayOfWeekAndHour(List<Member> membersWithoutTimeEntry, DayOfWeek currentDayOfWeek, int currentHour)
        {
            return membersWithoutTimeEntry
                .Where(member => member.SendEmailDays != null)
                .Where(member => ConverterBitMask.DaysOfWeekIntToListDayOfWeekAdaptive(member.SendEmailDays).Any(memberSendDayOfWeek => memberSendDayOfWeek.DayOfWeek == currentDayOfWeek))
                .Where(member => member.SendEmailTime == currentHour)
                .ToList();
        }

        #endregion

        #region ByProjects Settings.

        public async Task ByProjectSettings()
        {
            var todayDate = DateTime.Now;

            if (IsWorkDay(todayDate))
            {
                //CommonHelpers.SetRangeOfWorkWeekByDate(out var workWeekFirstDay, out var workWeekLastDay, todayDate);

                #region Active Private Projects with Notification Enable.

                var projectsPrivateWithNotification = GetPrivateProjectsWithNotificationEnable();
                if (projectsPrivateWithNotification.Any())
                {
                    foreach (var project in projectsPrivateWithNotification)
                    {
                        var membersWithoutTimeEntryForPrivateProjects = GetMembersWithoutTimeEntryForPrivateProjects(project, todayDate);

                        await SendNotificationsForMembers(membersWithoutTimeEntryForPrivateProjects, project, "Project");
                    }
                }

                #endregion

                #region Active Global Projects with Notification Enable.

                //var projectsGlobalWithNotification = GetGlobalProjectsWithNotificationEnable();
                //if (projectsGlobalWithNotification.Any())
                //{
                //    foreach (var project in projectsGlobalWithNotification)
                //    {
                //        var membersWithoutTimeEntry = GetMembersWithoutTimeEntryForGlobalProjects(project, todayDate);

                //        await SendNotificationsForMembers(membersWithoutTimeEntry, project, "Project Global");
                //    }
                //}

                #endregion

            }
        }

        private bool IsWorkDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        private List<DateTime> GetNotificationPeriodDays(DateTime todayDate, int projectNotificationDayCount)
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
            }
            while (projectNotificationDayCount > 0);

            NotificationPeriodFirstDay = notificationPeriodDays.Min(x => x.Date);
            NotificationPeriodLastDay = notificationPeriodDays.Max(x => x.Date);

            return notificationPeriodDays;
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

        private List<Project> GetPrivateProjectsWithNotificationEnable()
        {
            return Uow.ProjectRepository.LinkedCacheGetList()
                .Where(proj => proj.IsNotificationEnabled && proj.IsActive && proj.IsPrivate)
                .ToList();
        }

        private List<Project> GetGlobalProjectsWithNotificationEnable()
        {
            return Uow.ProjectRepository.LinkedCacheGetList()
                .Where(proj => proj.IsNotificationEnabled && proj.IsActive && !proj.IsPrivate)
                .ToList();
        }

        private List<Member> GetMembersWithoutTimeEntryForPrivateProjects(Project project, DateTime todayDate)
        {
            var currentHour = todayDate.TimeOfDay.Hours;

            var editionPeriodDays = GetNotificationPeriodDays(todayDate, project.NotificationDay).OrderByDescending(x => x.Date);

            var membersIdsWithTimeEntriesInNotificationPeriod = Uow.TimeEntryRepository.GetQueryWithIncludes()
                .Where(tEntry => editionPeriodDays.All(editionPeriodDay => tEntry.Member.TimeEntries.Where(proj => proj.ProjectId == project.Id).Select(tEntryDate => tEntryDate.Date.Date).Contains(editionPeriodDay)))
                .Select(memberId => memberId.MemberId)
                .Distinct()
                .ToArray();

            var membersWithoutTimeEntriesInNotificationPeriod = project.MemberProjectRoles
                .Where(mpr => !membersIdsWithTimeEntriesInNotificationPeriod.Contains(mpr.MemberId))
                .Select(mpr => mpr.Member)
                .Where(member => member.SendEmailTime == currentHour)
                .ToList();

            return membersWithoutTimeEntriesInNotificationPeriod;
        }

        private List<Member> GetMembersWithoutTimeEntryForGlobalProjects(Project project, DateTime todayDate)
        {
            var currentHour = todayDate.TimeOfDay.Hours;

            var editionPeriodDays = GetNotificationPeriodDays(todayDate, project.NotificationDay).OrderByDescending(x => x.Date);

            var membersIdsWithTimeEntriesInNotificationPeriod = Uow.TimeEntryRepository.GetQueryWithIncludes()
                .Where(tEntry => editionPeriodDays.All(editionPeriodDay => tEntry.Member.TimeEntries.Where(proj => proj.ProjectId == project.Id).Select(tEntryDate => tEntryDate.Date.Date).Contains(editionPeriodDay)))
                .Select(memberId => memberId.MemberId)
                .Distinct()
                .ToArray();

            var membersWithoutTimeEntriesInNotificationPeriod = Uow.MemberRepository.LinkedCacheGetList()
                .Where(member => !membersIdsWithTimeEntriesInNotificationPeriod.Contains(member.Id))
                .Where(member => member.SendEmailTime == currentHour)
                .ToList();
            
            return membersWithoutTimeEntriesInNotificationPeriod;
        }

        private async Task SendNotificationsForMembers(List<Member> membersByDayOfWeekAndHours, Project project, string subjectName)
        {
            if (membersByDayOfWeekAndHours.Any())
            {
                foreach (var member in membersByDayOfWeekAndHours)
                {
                    var dayOrDays = project.NotificationDay == 1 ? "day" : "days";

                    var linkToCreateTEntry = $"https://time.coral.team:1593/calendar/week;date={NotificationPeriodFirstDay.Month}-{NotificationPeriodFirstDay.Day}-{NotificationPeriodFirstDay.Year}";
                    var linkCallBackMain = "https://time.coral.team:1593/";

                    var dateFormatShort = new GetDateFormat().GetDateFormaDotNetShortById(member.DateFormatId);
                    var dateRangeShort = $"{NotificationPeriodFirstDay.ToString(dateFormatShort, CultureInfo.InvariantCulture)} - {NotificationPeriodLastDay.ToString(dateFormatShort, CultureInfo.InvariantCulture)}";

                    var emailSenderSimpleModel = new EmailSenderSimpleModel
                    {
                        Subject = $"Reminder to fill time entry {member.User.Email} by {subjectName} Settigs",
                        ToEmail = member.User.Email,
                        EmailText = $@"<p>Hello, {member.FullName}!<br>
                        <p>This is a friendly reminder that you haven’t filled your time entries for the last work <b>{project.NotificationDay}</b> {dayOrDays}:<br>
                        <p>({dateRangeShort}) on <b>{project.Name}</b> project.<br>
                        <p>Would you like to enter your time now? <a href=""{linkToCreateTEntry}"">Just click here to open CoralTime</a><br><br>
                        <p>Best wishes, <a href=""mailto:coraltime2017@yandex.ru"">CoralTime Team!</a>"
                    };

                    await EmailSenderSimple(_configuration, emailSenderSimpleModel);
                }
            }
        }

        #endregion
    }
} 
