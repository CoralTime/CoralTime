using System;
using System.Collections.Generic;

namespace CoralTime.Common.Models.Reports
{
    public interface IRequestReportsSettings
    {
        DateTime DateFrom { get; set; }

        DateTime DateTo { get; set; }

        int GroupById { get; set; }

        int?[] ClientIds { get; set; }

        int[] ProjectIds { get; set; }

        int[] MemberIds { get; set; }

        int[] ShowColumnIds { get; set; }
    }
}
