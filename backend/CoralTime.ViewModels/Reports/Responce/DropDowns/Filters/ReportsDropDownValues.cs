using CoralTime.ViewModels.Reports.Request.Grid;
using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportsDropDownValues : ReportsDropDowns
    {
        public ReportsDropDownValues()
        {
            UserDetails = new ReportsUserDetails();
            CustomQueries = new List<ReportsSettingsView>();
        }

        public ReportsUserDetails UserDetails { get; set; }

        public List<ReportsSettingsView> CustomQueries { get; set; }
    }
}
