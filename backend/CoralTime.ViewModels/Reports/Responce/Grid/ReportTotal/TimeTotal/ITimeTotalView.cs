namespace CoralTime.ViewModels.Reports
{
    public interface ITimeTotalView
    {
        int TimeActualTotal { get; set; }

        int? TimeEstimatedTotal { get; set; }
    }
}
