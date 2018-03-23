using System;
using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class ReportItemsView
    {
        public ReportItemsView()
        {
            ClientDisplayName = "Client";
            ProjectDisplayName = "Project";
            MemberDisplayName = "User";
            TaskDisplayName = "Task";
            DateDisplayName = "Date";
            NotesDisplayName = "Notes";
        }

        [JsonIgnore] public int? ClientId { get; set; }
        public string ClientName { get; set; }
        [JsonIgnore] public string ClientDisplayName { get; set; }

        [JsonIgnore] public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        [JsonIgnore] public string ProjectDisplayName { get; set; }

        [JsonIgnore] public int MemberId { get; set; }
        public string MemberName { get; set; }
        [JsonIgnore] public string MemberDisplayName { get; set; }

        [JsonIgnore] public int TaskId { get; set; }
        public string TaskName { get; set; }
        [JsonIgnore] public string TaskDisplayName { get; set; }

        public DateTime? Date { get; set; }
        [JsonIgnore] public string DateDisplayName { get; set; }

        public TimeValuesView TimeValues { get; set; }
        [JsonIgnore] public string TimeValuesDisplayName { get; set; }

        public string Notes { get; set; }
        [JsonIgnore] public string NotesDisplayName { get; set; }
    }
}