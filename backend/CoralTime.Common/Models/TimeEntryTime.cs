namespace CoralTime.Common.Models
{
    public class TimeEntryTime
    {
        public int Id { get; set; }

        public int? TimeFrom { get; set; }

        public int? TimeTo { get; set; }

        public int Time { get; set; }

        public int TimeTimerStart { get; set; } // It's time in seconds after 00.00, that display time when Timer is run.
    }
}