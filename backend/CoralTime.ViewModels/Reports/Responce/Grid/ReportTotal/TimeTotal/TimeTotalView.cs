using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class TimeTotalView : ITimeTotalView
    {
        public TimeTotalView()
        {
            TimeEstimatedTotal = 0;
            DisplayNameTimeActualTotal = "Total";
            DisplayNameTimeEstimatedTotal = "Total";
        }

        public int TimeActualTotal { get; set; }
        public int? TimeEstimatedTotal { get; set; }

        [JsonIgnore] public string DisplayNameTimeActualTotal { get; set; }
        [JsonIgnore] public string DisplayNameTimeEstimatedTotal { get; set; }
    }
}
