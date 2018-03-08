using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportsTotalGridProjectsView: IReportsTotalGridView<ReportTotalForGridProjectView>
    {
        public ReportsTotalGridProjectsView()
        {
            ReportsGridView = new List<ReportTotalForGridProjectView>
            {
                new ReportTotalForGridProjectView
                {
                    Items = new List<ReportsGridItemsView>()
                }
            };
        }

        public int TotalEstimatedTime { get; set; }

        public int TotalActualTime { get; set; }

        public IEnumerable<ReportTotalForGridProjectView> ReportsGridView { get; set; }
    }
}
