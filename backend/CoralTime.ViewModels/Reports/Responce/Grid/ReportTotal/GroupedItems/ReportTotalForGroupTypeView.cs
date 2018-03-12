namespace CoralTime.ViewModels.Reports
{
    public class ReportTotalForGroupTypeView : ReportTotalForView
    {
        public ReportTotalForGroupTypeView(int groupById)
        {
            GroupByType = new ReportGroupByType(groupById);
        }

        public ReportGroupByType GroupByType { get; set; }
    }
}