using System.ComponentModel.DataAnnotations;
using System;

namespace CoralTime.ViewModels.TimeEntries
{
    public class TimeEntryView
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public int TaskTypesId { get; set; }

        public string TaskName { get; set; }

        public DateTime Date { get; set; }

        #region Time values.

        public int Time { get; set; }

        public int? PlannedTime { get; set; }

        public int? TimeFrom { get; set; }

        public int? TimeTo { get; set; }
        
        public int TimeTimerStart { get; set; } // It's time in seconds after 00.00, that display time when Timer is run.

        #endregion

        public string Description { get; set; }

        public bool IsFromToShow { get; set; }

        public bool IsProjectActive { get; set; }

        public bool IsTaskTypeActive { get; set; }

        public bool IsUserManagerOnProject { get; set; }

        public int Color { get; set; }
    }
}