using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportTotalView : ReportTotalForGroupTypeView
    {
        public ReportTotalView(int? groupById, int[] showColumnIds, int? dateFormatId, DateTime? dateFrom, DateTime? dateTo)
            : base(groupById,showColumnIds,dateFormatId)
        { 
            TimeTotal = new TimeTotalView();

            GroupedItems = new List<ReportTotalForGroupTypeView>
            {
                new ReportTotalForGroupTypeView(groupById, showColumnIds, dateFormatId)
            };

            PeriodCell =  new ReportPeriodView(dateFrom, dateTo);
        }

        public TimeTotalView TimeTotal { get; }

        [JsonIgnore] public ReportPeriodView PeriodCell { get; set; }

        public List<ReportTotalForGroupTypeView> GroupedItems { get; set; }
    }
}