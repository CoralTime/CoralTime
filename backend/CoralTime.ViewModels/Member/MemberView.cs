using CoralTime.ViewModels.Member.MemberNotificationView;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Member
{
    public class MemberView : IMemberPersonalInfoView, IMemberNotificationView, IMemberPreferencesView
    {
        [Key]        
        public int Id { get; set; }

        public string FullName { get; set; }

        public int DefaultProjectId { get; set; }

        public int DefaultTaskId { get; set; }

        public string TimeZone { get; set; }

        public string UrlIcon { get; set; }

        public int WeekStart { get; set; }

        public int DateFormatId { get; set; }

        public string DateFormat { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsManager { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<ProjectView> Projects { get; set; }

        public int? ProjectsCount { get; set; }

        public int TimeFormat { get; set; }

        public int SendEmailTime { get; set; }

        public bool IsWeeklyTimeEntryUpdatesSend { get; set; }

        public string SendEmailDays { get; set; }
    }
}