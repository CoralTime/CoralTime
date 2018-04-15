using System;

namespace CoralTime.ViewModels.Reports.Request.Grid
{
    public class ReportsSettingsView : IReportsSettingsView
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? DateStaticId { get; set; }

        public int? GroupById { get; set; }

        public int?[] ClientIds { get; set; }

        public int[] ProjectIds { get; set; }

        public int[] MemberIds { get; set; }

        public int[] ShowColumnIds { get; set; }

        public int? QueryId { get; set; }

        public string QueryName{ get; set; }
    }
}
