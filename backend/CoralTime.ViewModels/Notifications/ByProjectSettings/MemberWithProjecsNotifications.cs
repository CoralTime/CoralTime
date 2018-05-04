using System.Collections.Generic;

namespace CoralTime.ViewModels.Notifications
{
    public class MemberWithProjecsNotifications
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
}
