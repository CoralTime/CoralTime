﻿using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns
{
    public class ReportDropDowns
    {
        public ReportDropDowns()
        {
            Filters = new List<ReportClientView>();
        }

        public List<ReportClientView> Filters { get; set; }

        public ReportCommonDropDownsView[] GroupBy { get; set; }

        public ReportCommonDropDownsView[] ShowColumns { get; set; }

        public ReportDropDownsDateStaticView[] DateStatic { get; set; }
    }
}
