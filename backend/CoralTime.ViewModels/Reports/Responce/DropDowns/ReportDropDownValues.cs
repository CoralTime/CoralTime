using CoralTime.ViewModels.Reports.Request.Grid;
using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportDropDownValues : ReportDropDowns
    {
        public ReportDropDownValues()
        {
            UserDetails = new ReportUserDetails();
            CustomQueries = new List<ReportsSettingsView>();
        }

        public ReportUserDetails UserDetails { get; set; }

        public List<ReportsSettingsView> CustomQueries { get; set; }
    }
}
