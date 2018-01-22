﻿using CoralTime.ViewModels.Reports.Request;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;
using System.Collections.Generic;

namespace CoralTime.Common.Models.Reports.Responce.DropDowns.Filters
{
    public class ReportsDropDowns
    {
        public ReportsDropDowns()
        {
            Filters = new List<ReportClientView>();
            GroupBy = new List<ReportsDropDownGroupBy>();
            //ShowColumns = new ShowColumnModel[0];
        }

        public List<ReportClientView> Filters { get; set; }

        public List<ReportsDropDownGroupBy> GroupBy { get; set; }

        public ShowColumnModel[] ShowColumns { get; set; }
    }
}
