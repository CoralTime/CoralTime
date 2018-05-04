using System;

namespace CoralTime.ViewModels.Notifications
{
    public class ProjectEditionDays
    {
        public DateTime[] EditionDays { get; set; }

        public DateTime NotificationPeriodFirstDay { get; set; }

        public DateTime NotificationPeriodLastDay { get; set; }

        public ProjectEditionDays()
        {
            EditionDays = new DateTime[0];
        }
    }
}
