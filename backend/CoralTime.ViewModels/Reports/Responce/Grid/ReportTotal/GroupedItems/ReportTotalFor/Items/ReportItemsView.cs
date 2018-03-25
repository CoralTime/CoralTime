using System;
using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class ReportItemsView: ReportGroupByTypeDisplayNames
    {
        private ReportItemsView() 
            : base(null,null) { }

        public ReportItemsView(int? groupById, int[] showColumnIds)
            : base(groupById, showColumnIds) { }

        [JsonIgnore] public int? ClientId { get; set; }
        public string ClientName { get; set; }

        [JsonIgnore] public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        
        [JsonIgnore] public int MemberId { get; set; }
        public string MemberName { get; set; }

        [JsonIgnore] public int TaskId { get; set; }
        public string TaskName { get; set; }

        public DateTime? Date { get; set; }

        public TimeValuesView TimeValues { get; set; }

        public string Notes { get; set; }
    }
}