using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportsDropDownsView 
    {
        public ReportsDropDownsView()
        {
            Values = new ReportsDropDownValues();
            ValuesDefaultQuery = new ReportsSettingsView();
        }

        public ReportsDropDownValues Values { get; set; }

        public ReportsSettingsView ValuesDefaultQuery { get; set; }
    }
}