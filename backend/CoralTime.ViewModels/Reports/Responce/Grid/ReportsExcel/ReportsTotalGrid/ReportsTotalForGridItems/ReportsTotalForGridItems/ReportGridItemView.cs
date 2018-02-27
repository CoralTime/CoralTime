using System;
using CoralTime.ViewModels.Reports.Responce.ReportsGrid;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsGridItemsView : IReportsGridItemsView
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public int TaskId { get; set; }

        public string TaskName { get; set; }

        public DateTime Date { get; set; }

        public string TimeFrom{ get; set; }

        public string TimeTo { get; set; }

        public int TimeActual { get; set; }

        public int TimeEstimated { get; set; }

        public string Description { get; set; }
    }
}