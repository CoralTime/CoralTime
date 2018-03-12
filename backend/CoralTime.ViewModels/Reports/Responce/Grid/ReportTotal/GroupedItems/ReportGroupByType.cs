using Newtonsoft.Json;
using System;

namespace CoralTime.ViewModels.Reports
{
    public class ReportGroupByType
    {
        private ReportGroupByType() { }

        public ReportGroupByType(int groupById)
        {
            GroupByTypeId = groupById;

            ProjectName = null;
            MemberName = null;
            Date = null;
            ClientName = null;    
        }

        [JsonIgnore] public int? GroupByTypeId { get; set; }
        [JsonIgnore] public string GroupByTypeDisplayName { get; set; }

        [JsonIgnore] public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        [JsonIgnore] public int MemberId { get; set; }
        public string MemberName { get; set; }

        public DateTime? Date { get; set; }

        [JsonIgnore] public int ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
