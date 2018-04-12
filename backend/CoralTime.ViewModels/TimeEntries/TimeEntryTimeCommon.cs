using CoralTime.ViewModels.Reports;

namespace CoralTime.ViewModels.TimeEntries
{
    public class TimeValuesView: ITimeValuesView
    {
        public int? TimeFrom { get; set; }

        public int? TimeTo { get; set; }

        public int TimeActual { get; set; }

        public int? TimeEstimated { get; set; }
    }

    public class TimeOptions
    {
        public int TimeTimerStart { get; set; }

        public bool IsFromToShow { get; set; }
    }
}
