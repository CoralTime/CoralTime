using System;
using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems
{
    public class ReportGroupByType
    {
        [JsonIgnore] public int[] ShowColumnIds { get; set; }
        [JsonIgnore] public int? GroupByTypeId { get; set; }
        [JsonIgnore] public string GroupByTypeDisplayName { get; set; }
        [JsonIgnore] public string GroupByTypeDisplayNameValue { get; set; }

        private ReportGroupByType() { }

        public ReportGroupByType(int? groupById, int[] showColumnIds)
        {
            GroupByTypeId = groupById;
            ShowColumnIds = showColumnIds;

            ProjectName = null;
            MemberName = null;
            Date = null;
            ClientName = null;    
        }

        [JsonIgnore] public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        [JsonIgnore] public int MemberId { get; set; }
        public string MemberName { get; set; }
        
        public string MemberUrlIcon { get; set; }

        public DateTime? Date { get; set; }

        [JsonIgnore] public int ClientId { get; set; }
        public string ClientName { get; set; }

        public int? WorkingHoursPerDay { get; set; }
    }
}
