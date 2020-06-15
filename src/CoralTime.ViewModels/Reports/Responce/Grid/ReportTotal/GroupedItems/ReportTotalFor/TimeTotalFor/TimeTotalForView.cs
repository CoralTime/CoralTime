﻿using System.Text.Json.Serialization;

namespace CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems.ReportTotalFor.TimeTotalFor
{
    public class TimeTotalForView
    {
        [JsonIgnore] public string DisplayNameTimeActualTotalFor { get; }
        [JsonIgnore] public string DisplayNameTimeEstimatedTotalFor { get; set; }

        public TimeTotalForView()
        {
            TimeEstimatedTotalFor = 0;
            DisplayNameTimeActualTotalFor = "Total For ";
            DisplayNameTimeEstimatedTotalFor = "Total For ";
        }

        public int TimeActualTotalFor { get; set; }

        public int? TimeEstimatedTotalFor { get; set; }
    }
}
