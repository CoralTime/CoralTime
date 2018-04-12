using System;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        public static bool IsTimeEntryLocked(DateTime timeEntryDateEditing, int daysAfterLock, LockTimePeriod lockPeriod)
        {
            var today = DateTime.Today;

            switch (lockPeriod)
            {
                case LockTimePeriod.Week:
                {
                    SetRangeOfThisWeekByDate(out var weekByTodayFirstDate, out var weekByTodayLastDate, today);

                    var lockDateLastDate = weekByTodayFirstDate.AddDays(daysAfterLock);

                    var isTodayInLockPeriodAtThisWeek = today <= lockDateLastDate;

                    var todayDateAtPreviousWeek = today.AddDays(-14);
                    var todayDateAtThisWeek = today.AddDays(-7);

                    var editingWeekDay = isTodayInLockPeriodAtThisWeek ? todayDateAtPreviousWeek : todayDateAtThisWeek;

                    SetRangeOfThisWeekByDate(out var editingWeekFirstDate, out var editingWeekLastDate, editingWeekDay);

                    return timeEntryDateEditing <= editingWeekLastDate;
                }

                case LockTimePeriod.Month:
                {
                    SetRangeOfThisMonthByDate(out var monthByTodayFirstDate, out var monthByTodayLastDate, today);

                    var lockDateLastDate = monthByTodayFirstDate.AddDays(daysAfterLock);

                    var isInLockPeriod = today <= lockDateLastDate;

                    var todayDateAtPreviousMonth = today.AddMonths(-2);
                    var todayDateAtThisMonth = today.AddMonths(-1);

                    var editingMonthDay = isInLockPeriod ? todayDateAtPreviousMonth : todayDateAtThisMonth;

                    SetRangeOfThisMonthByDate(out var editingMonthFirstDate, out var editingMonthLastDate, editingMonthDay);

                    return timeEntryDateEditing <= editingMonthLastDate;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}