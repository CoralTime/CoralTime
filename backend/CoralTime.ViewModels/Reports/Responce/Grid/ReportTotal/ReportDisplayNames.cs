using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class ReportDisplayNames
    {
        public ReportDisplayNames()
        {
            DisplayNameClient = "Client";
            DisplayNameProject = "Project";
            DisplayNameMember = "User";
            DisplayNameDate = "Date";

            DisplayNameTimeFrom = "Start";
            DisplayNameTimeTo = "Finish";
            DisplayNameTimeActual = "Act. Hours";
            DisplayNameTimeEstimated = "Est. Hours";
            DisplayNameTask = "Task";
            DisplayNameNotes = "Notes";
        }

        [JsonIgnore] public string DisplayNameDate { get; set; }
        [JsonIgnore] public string DisplayNameProject { get; set; }
        [JsonIgnore] public string DisplayNameClient { get; set; }
        [JsonIgnore] public string DisplayNameMember { get; set; }

        [JsonIgnore] public string DisplayNameTimeFrom { get; set; }
        [JsonIgnore] public string DisplayNameTimeTo { get; set; }
        [JsonIgnore] public string DisplayNameTimeActual { get; set; }
        [JsonIgnore] public string DisplayNameTimeEstimated { get; set; }

        [JsonIgnore] public string DisplayNameTask { get; set; }
        [JsonIgnore] public string DisplayNameNotes { get; set; }
    }
}
