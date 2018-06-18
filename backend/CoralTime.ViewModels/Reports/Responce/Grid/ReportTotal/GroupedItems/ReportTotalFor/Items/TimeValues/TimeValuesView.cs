namespace CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems.ReportTotalFor.Items.TimeValues
{
    public class TimeValuesView : ITimeValuesView
    {
        public int? TimeFrom { get; set; }

        public int? TimeTo { get; set; }

        public int TimeActual { get; set; }

        public int? TimeEstimated { get; set; }
    }
}
