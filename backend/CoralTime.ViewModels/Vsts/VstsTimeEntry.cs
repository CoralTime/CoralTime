using System;

namespace CoralTime.ViewModels.Vsts
{
    public class VstsTimeEntry
    {
        public string ProjectName { get; set; }

        public string TaskName { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public int TimeActual { get; set; }

        public int? TimeEstimated { get; set; }

        public string UserName { get; set; }
    }
}