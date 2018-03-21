namespace CoralTime.ViewModels.Reports
{
    public interface ITimeTotalForView
    {
        int TimeActualTotalFor { get; set; }

        int? TimeEstimatedTotalFor { get; set; }
    }
}