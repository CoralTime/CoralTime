using System;

namespace CoralTime.ViewModels.Reports.Responce.ReportsGrid
{
    public interface IReportsGridItemsView
    {
        string ClientId { get; set; }

        string ClientName { get; set; }

        int ProjectId { get; set; }

        string ProjectName { get; set; }

        int MemberId { get; set; }

        string MemberName { get; set; }

        int TaskId { get; set; }

        string TaskName { get; set; }

        DateTime Date { get; set; }

        string TimeFrom { get; set; }

        string TimeTo { get; set; }

        int TimeActual { get; set; }

        int TimeEstimated { get; set; }

        string Description { get; set; }
    }
}
