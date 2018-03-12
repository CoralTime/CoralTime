using CoralTime.ViewModels.Reports.Responce.DropDowns.GroupBy;
using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportsDropDowns
    {
        public ReportsDropDowns()
        {
            Filters = new List<ReportClientView>();
            GroupBy = new List<ReportsDropDownGroupBy>();
            //ShowColumns = new ShowColumnModel[0];
        }

        public List<ReportClientView> Filters { get; set; }

        public List<ReportsDropDownGroupBy> GroupBy { get; set; }

        public ShowColumnModel222[] ShowColumns { get; set; }
    }
}
