using System;

namespace CoralTime.ViewModels.Reports.Request.Grid
{
    public class RequestReportsGrid : IRequestReportsSettings
    {
        public int DateFormatId { get; set; }

        public int? FileTypeId { get; set; }


        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? GroupById { get; set; }

        public int?[] ClientIds { get; set; }

        public int[] ProjectIds { get; set; }

        public int[] MemberIds { get; set; }

        public int[] ShowColumnIds { get; set; }
    }
}