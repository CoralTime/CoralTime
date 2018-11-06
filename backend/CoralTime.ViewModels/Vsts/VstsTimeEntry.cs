using CoralTime.ViewModels.TimeEntries;
using System;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsTimeEntry
    {
        public string ProjectId { get; set; }

        public string TaskId { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string MemberId { get; set; }

        public int VstsEntityId { get; set; }

        public TimeOptions TimeOptions { get; set; }

        public TimeValuesView TimeValues { get; set; }
    }
}