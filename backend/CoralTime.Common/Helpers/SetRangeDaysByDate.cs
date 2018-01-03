using System;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        public static void SetRangeOfWeekByDate(out DateTime weekDateStart, out DateTime weekDateEnd, DateTime dayOfWeek)
        {
            var startOfWeek = DayOfWeek.Monday;

            int diff = dayOfWeek.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            weekDateStart = dayOfWeek.AddDays(-1 * diff).Date;
            weekDateEnd = weekDateStart.AddDays(7).AddMilliseconds(-1);
        }

        public static void SetRangeOfMonthByDate(out DateTime monthDateStart, out DateTime monthDateEnd, DateTime day)
        {
            monthDateStart = new DateTime(day.Year, day.Month, 1);
            monthDateEnd = new DateTime(day.Year, day.Month, 1).AddMonths(1).AddMilliseconds(-1);
        }

        public static void SetRangeOfWorkWeekByDate(out DateTime workWeekDateStart, out DateTime workWeekDateEnd, DateTime dayOfWeek)
        {
            SetRangeOfWeekByDate(out workWeekDateStart, out workWeekDateEnd, dayOfWeek);

            workWeekDateEnd = workWeekDateEnd.AddDays(-2);
        }
    }
}