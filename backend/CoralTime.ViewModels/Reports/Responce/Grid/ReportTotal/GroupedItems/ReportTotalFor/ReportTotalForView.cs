using System.Collections.Generic;
using CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems.ReportTotalFor.Items;
using CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems.ReportTotalFor.TimeTotalFor;

namespace CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems.ReportTotalFor
{
    public class ReportTotalForView : ReportItemsView
    {
        public ReportTotalForView(int? groupById, int[] showColumnIds) 
            : base (groupById,showColumnIds)
        {
            TimeTotalFor = new TimeTotalForView();
            Items = new List<ReportItemsView>();
        }

        public TimeTotalForView TimeTotalFor { get; set; }

        public List<ReportItemsView> Items { get; set; }
    }
}