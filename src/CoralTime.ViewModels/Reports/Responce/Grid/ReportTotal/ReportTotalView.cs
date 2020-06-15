﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems;
using CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.TimeTotal;

namespace CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal
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