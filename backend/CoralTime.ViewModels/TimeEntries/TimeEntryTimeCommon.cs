namespace CoralTime.ViewModels.TimeEntries
{
    public class TimeEntryTimeView
    {
        public int Time { get; set; }

        public int? TimeFrom { get; set; }

        public int? TimeTo { get; set; }

        public int TimeTimerStart { get; set; }

        public bool IsFromToShow { get; set; }
    }
}
