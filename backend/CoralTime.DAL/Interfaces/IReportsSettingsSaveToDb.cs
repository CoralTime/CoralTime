using System;

namespace CoralTime.DAL.Interfaces
{
    public interface IReportsSettingsSaveToDb
    {
        DateTime? DateFrom { get; set; }

        DateTime? DateTo { get; set; }

        int[] ProjectIds { get; set; }

        int[] MemberIds { get; set; }

        int?[] ClientIds { get; set; }

        int? GroupById { get; set; }

        int[] ShowColumnIds { get; set; }
    }
}
