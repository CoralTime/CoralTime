using CoralTime.ViewModels.Member;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoralTime.ViewModels.Projects
{
    public class ManagerProjectsView
    {
        [Key]
        public int Id { get; set; }

        public string ClientName { get; set; }

        public string Name { get; set; }

        public int? ClientId { set; get; }

        public int DaysBeforeStopEditTimeEntries { set; get; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int MembersCount { get; set; }

        public int TasksCount { get; set; }

        public bool IsPrivate { get; set; }

        public int Color { get; set; }

        public int LockPeriod { get; set; }

        public int NotificationDay { get; set; }

        public bool IsTimeLockEnabled { get; set; }
        
        public bool IsNotificationEnabled { get; set; }

        public IEnumerable<MemberView> Members { get; set; }
    }
}