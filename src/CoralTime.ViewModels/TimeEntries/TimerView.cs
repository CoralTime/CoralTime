using System;
using System.Collections.Generic;
using System.Text;

namespace CoralTime.ViewModels.TimeEntries
{
    public class TimerView
    {
        public TimeEntryView TimeEntry { get; set; }
        public int? TrackedTime { get; set; }
    }
}
