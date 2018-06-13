namespace CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal.GroupedItems.ReportTotalFor.Items.TimeValues
{
    public interface ITimeValuesView
    {
        int? TimeFrom { get; set; }

        int? TimeTo { get; set; }

        int TimeActual { get; set; }

        int? TimeEstimated { get; set; }
    }
}
