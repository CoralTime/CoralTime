namespace CoralTime.ViewModels.Reports
{
    public interface ITimeValuesView
    {
        int? TimeFrom { get; set; }

        int? TimeTo { get; set; }

        int TimeActual { get; set; }

        int? TimeEstimated { get; set; }
    }
}
