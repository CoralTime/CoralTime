using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports
{
    public class ReportTotalView 
    {
        [JsonIgnore] public int? GroupByTypeId { get; }
        [JsonIgnore] public int[] ShowColumnIds { get; }
        [JsonIgnore] public int? DateFormatId { get; }

        public ReportTotalView(int groupById, int[] showColumnIds, int? dateFormatId, DateTime? dateFrom, DateTime? dateTo)
        {
            TimeTotal = new TimeTotalView();

            GroupedItems = new List<ReportTotalForGroupTypeView>
            {
                new ReportTotalForGroupTypeView(groupById, showColumnIds, dateFormatId)
            };

            GroupByTypeId = groupById;
            ShowColumnIds = showColumnIds;
            DateFormatId = dateFormatId;

            PeriodCell =  new ReportPeriodView(dateFrom, dateTo);
        }

        public TimeTotalView TimeTotal { get; }

        [JsonIgnore] public ReportPeriodView PeriodCell { get; set; }

        public List<ReportTotalForGroupTypeView> GroupedItems { get; set; }
    }
}