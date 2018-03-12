using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class TimeTotalForView : ITimeTotalForView
    {
        public TimeTotalForView()
        {
            TimeEstimatedTotalFor = 0;
            TimeActualTotalForDisplayName = "Total For Actual Time";
            TimeEstimatedTotalForDisplayName = "Total For Estimated Time";
        }

        public int TimeActualTotalFor { get; set; }
        [JsonIgnore] public string TimeActualTotalForDisplayName { get; set; }

        public int? TimeEstimatedTotalFor { get; set; }
        [JsonIgnore] public string TimeEstimatedTotalForDisplayName { get; set; }
    }
}
