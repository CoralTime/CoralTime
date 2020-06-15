namespace CoralTime.DAL.Models.TimeValues
{
    public interface ITimeValues
    {
        int TimeFrom { get; set; }

        int TimeTo { get; set; }

        int TimeActual { get; set; }

        int TimeEstimated { get; set; }
    }
}
