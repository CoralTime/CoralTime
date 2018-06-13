using System.Collections.Generic;
using CoralTime.ViewModels.Reports.Request.ReportsSettingsView;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns
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
