using Newtonsoft.Json;

namespace CoralTime.ViewModels.Reports
{
    public class ReportTotalForGroupTypeView : ReportTotalForView
    {
        [JsonIgnore] public int? DateFormatId { get; }

        public ReportTotalForGroupTypeView(int? groupById, int[] showColumnIds, int? dateFormatId)
            : base(groupById, showColumnIds)
        {
            GroupByType = new ReportGroupByType(groupById, showColumnIds);
            DateFormatId = dateFormatId;
        }

        public ReportGroupByType GroupByType { get; }
    }
}