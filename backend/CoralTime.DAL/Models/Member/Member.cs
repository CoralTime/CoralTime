using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoralTime.Common.Constants;

namespace CoralTime.DAL.Models
{
    public class Member : LogChanges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        public List<TimeEntry> TimeEntries { get; set; }

        public List<ReportsSettings> ReportsSettings { get; set; }

        public MemberImage MemberImage { get; set; }

        public int DefaultProjectId { get; set; }

        public int DefaultTaskId { get; set; }

        public Constants.WeekStart WeekStart { get; set; }

        public int DateFormatId { get; set; }

        public List<MemberProjectRole> MemberProjectRoles { get; set; }

        public int TimeFormat { get; set; }

        public int SendEmailTime { get; set; }

        public bool IsWeeklyTimeEntryUpdatesSend { get; set; }

        public int? SendEmailDays { get; set; }
    }
}