using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class TimeTotalView : ITimeTotalView
    {
        public TimeTotalView()
        {
            TimeEstimatedTotal = 0;
            TimeActualTotalDisplayName = "Total Actual Time";
            TimeEstimatedTotalDisplayName = "Total Estimated Time";
        }

        public int TimeActualTotal { get; set; }
        [JsonIgnore] public string TimeActualTotalDisplayName { get; set; }

        public int? TimeEstimatedTotal { get; set; }
        [JsonIgnore] public string TimeEstimatedTotalDisplayName { get; set; }
    }
}
