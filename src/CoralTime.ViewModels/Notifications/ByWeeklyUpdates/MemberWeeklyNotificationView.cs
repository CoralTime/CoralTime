using System.Collections.Generic;

namespace CoralTime.ViewModels.Notifications.ByWeeklyUpdates
{
    public class MemberWeeklyNotificationView
    {
        public int Id { get; set; }

        public string FullName { get; set; }
    }

    public class MemberWeeklyNotificationByDayOfWeekView
    {
        public string WeekStartName { get; set; }

        public List<MemberWeeklyNotificationView> Members { get; set; }
    }
}
