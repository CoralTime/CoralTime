using System;

namespace CoralTime.ViewModels.Reports
{
    public interface IReportsSettingsView
    {
        DateTime? DateFrom { get; set; }

        DateTime? DateTo { get; set; }

        int? DateStaticId { get; set; }

        int? GroupById { get; set; }

        int?[] ClientIds { get; set; }

        int[] ProjectIds { get; set; }

        int[] MemberIds { get; set; }

        int[] ShowColumnIds { get; set; }

        int? QueryId { get; set; }

        string QueryName { get; set; }
    }
}
