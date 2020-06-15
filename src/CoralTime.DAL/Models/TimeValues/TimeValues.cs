namespace CoralTime.DAL.Models.TimeValues
{
    public class TimeValues : ITimeValues
    {
        public int TimeFrom { get; set; }

        public int TimeTo { get; set; }

        public int TimeActual { get; set; }

        public int TimeEstimated { get; set; }
    }
}
