using System;

namespace CoralTime.ViewModels.Reports.Responce
{
    public class ReportsSettings : IReportsSettings
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string FilterProjectIds { get; set; }

        public string FilterMemberIds { get; set; }

        public string FilterClientIds { get; set; }

        public int? GroupById { get; set; }

        public string FilterShowColumnIds { get; set; }
    }
}
