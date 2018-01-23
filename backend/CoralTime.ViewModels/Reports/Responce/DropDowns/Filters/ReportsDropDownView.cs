using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportsDropDownsView 
    {
        public ReportsDropDownsView()
        {
            Values = new ReportsDropDownValues();
            ValuesSaved = new RequestReportsSettings();
        }

        public ReportsDropDownValues Values { get; set; }

        public RequestReportsSettings ValuesSaved { get; set; }
    }
}