using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoralTime.Common.Constants;

namespace CoralTime.DAL.Models
{
    public class Project : LogChanges, IInitializeByName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public int DaysBeforeStopEditTimeEntries { get; set; }

        public bool IsTimeLockEnabled { get; set; }

        public bool IsNotificationEnabled { get; set; }

        public List<TimeEntry> TimeEntries { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsActive { get; set; }

        public bool IsActiveBeforeArchiving { get; set; }

        public int Color { get; set; }

        public List<MemberProjectRole> MemberProjectRoles { get; set; }

        public List<TaskType> TaskTypes { get; set; }

        public Constants.LockTimePeriod LockPeriod { get; set; }

        public int NotificationDay { get; set; }
    }
}