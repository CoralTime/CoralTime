using System.ComponentModel.DataAnnotations;
using System;

namespace CoralTime.ViewModels.TimeEntries
{
    public class TimeEntryView : TimeEntryTimeView
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public int TaskTypesId { get; set; }

        public string TaskName { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public int? PlannedTime { get; set; }

        public bool IsProjectActive { get; set; }

        public bool IsTaskTypeActive { get; set; }

        public bool IsUserManagerOnProject { get; set; }

        public int Color { get; set; }
    }
}