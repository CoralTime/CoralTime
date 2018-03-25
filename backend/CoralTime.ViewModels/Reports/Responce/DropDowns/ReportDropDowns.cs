using CoralTime.ViewModels.Reports.Responce.DropDowns.GroupBy;
using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportDropDowns
    {
        public ReportDropDowns()
        {
            Filters = new List<ReportClientView>();
            GroupBy = new List<ReportDropDownGroupBy>();
            //ShowColumns = new ShowColumnModel[0];
        }

        public List<ReportClientView> Filters { get; set; }

        public List<ReportDropDownGroupBy> GroupBy { get; set; }

        public ReportShowColumnModel[] ShowColumns { get; set; }
    }
}
