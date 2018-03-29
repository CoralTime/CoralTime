using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportDropDownsView 
    {
        public ReportDropDownsView()
        {
            Values = new ReportDropDownValues();
            CurrentQuery = new ReportsSettingsView();
        }

        public ReportDropDownValues Values { get; set; }

        public ReportsSettingsView CurrentQuery { get; set; }
    }
}