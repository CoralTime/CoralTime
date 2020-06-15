using CoralTime.ViewModels.TimeEntries;
using System;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsTimeEntry
    {
        public int? ProjectId { get; set; }

        public int? TaskId { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public int? MemberId { get; set; }

        public string WorkItemId { get; set; }

        public TimeOptions TimeOptions { get; set; }

        public TimeValuesView TimeValues { get; set; }

        public string MemberName { get; set; }

        public string TaskName { get; set; }
    }
}