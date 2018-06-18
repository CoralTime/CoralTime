using CoralTime.ViewModels.Reports.Request.ReportsSettingsView;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns
{
    public class ReportDropDownView 
    {
        public ReportDropDownView()
        {
            Values = new ReportDropDownValues();
            CurrentQuery = new ReportsSettingsView();
        }

        public ReportDropDownValues Values { get; set; }

        public ReportsSettingsView CurrentQuery { get; set; }
    }
}