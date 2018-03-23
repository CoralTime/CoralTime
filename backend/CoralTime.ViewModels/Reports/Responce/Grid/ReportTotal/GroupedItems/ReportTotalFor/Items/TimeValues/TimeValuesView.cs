using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class TimeValuesView : TimeValuesDisplayName, ITimeValuesView
    {
        public int? TimeFrom { get; set; }

        public int? TimeTo { get; set; }

        public int TimeActual { get; set; }

        public int? TimeEstimated { get; set; }
    }

    public class TimeValuesDisplayName
    {
        public TimeValuesDisplayName()
        {
            TimeFromDisplayName = "Start";
            TimeToDisplayName = "Finish";
            TimeActualDisplayName = "Actual Hours";
            TimeEstimatedDisplayName = "Estimated Hours";
        }

        [JsonIgnore] public string TimeEstimatedDisplayName { get; set; }

        [JsonIgnore] public string TimeActualDisplayName { get; set; }

        [JsonIgnore] public string TimeToDisplayName { get; set; }

        [JsonIgnore] public string TimeFromDisplayName { get; set; }
    }
}
