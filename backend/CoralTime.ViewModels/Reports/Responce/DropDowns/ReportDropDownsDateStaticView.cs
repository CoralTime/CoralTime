using System;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.GroupBy
{
    public class ReportDropDownsDateStaticView : ReportCommonDropDownsView
    {
        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }

    public class ReportDropDownsDateStaticExtendView
    {
        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int? DateStaicid { get; set; }

        public ReportDropDownsDateStaticView[] DateStatic { get; set; }
    }
}