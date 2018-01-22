using System;

namespace CoralTime.Common.Models.Reports.Responce
{
    public interface IReportsSettings
    {
        DateTime DateFrom { get; set; }

        DateTime DateTo { get; set; }

        int GroupById { get; set; }

        string ClientIds { get; set; }

        string ProjectIds { get; set; }

        string MemberIds { get; set; }

        string ShowColumnIds { get; set; }
    }
}
