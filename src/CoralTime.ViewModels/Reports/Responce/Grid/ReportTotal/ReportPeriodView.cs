using System;
using System.Text.Json.Serialization;

namespace CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal
{
    public class ReportPeriodView
    {
        [JsonIgnore]  public DateTime? DateFrom { get; set; }

        [JsonIgnore] public DateTime? DateTo { get; set; }

        private ReportPeriodView() { }

        public ReportPeriodView(DateTime? dateFrom, DateTime? dateTo)
        {
            DisplayNamePeriod = "Period: ";
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public string DisplayNamePeriod { get; }

        public string DisplayNamePeriodValue { get; set; }
    }
}
