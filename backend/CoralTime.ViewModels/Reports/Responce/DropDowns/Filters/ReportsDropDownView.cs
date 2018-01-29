using System.Collections.Generic;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportsDropDownsView 
    {
        public ReportsDropDownsView()
        {
            Values = new ReportsDropDownValues();
            ValuesSaved = new List<ReportsSettingsView>();
        }

        public ReportsDropDownValues Values { get; set; }

        public List<ReportsSettingsView> ValuesSaved { get; set; }
    }
}