using CoralTime.Common.Models.Reports.Request.Grid;

namespace CoralTime.Common.Models.Reports.Responce.DropDowns.Filters
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