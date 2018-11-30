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
                    var thisWeek = GetRangeOfThisWeek(today);

                    var lockDateLastDate = thisWeek.DateFrom.AddDays(daysAfterLock);

                    var isTodayInLockPeriodAtThisWeek = today <= lockDateLastDate;

                    var todayDateAtPreviousWeek = today.AddDays(-14);
                    var todayDateAtThisWeek = today.AddDays(-7);

                    var editingWeekDay = isTodayInLockPeriodAtThisWeek ? todayDateAtPreviousWeek : todayDateAtThisWeek;

                    var editingWeek = GetRangeOfWeekByDate(editingWeekDay);

                    return timeEntryDateEditing <= editingWeek.DateTo;
                }

                case LockTimePeriod.Month:
                {
                    var thisMonth = GetRangeOfThisMonth(today);

                    var lockDateLastDate = thisMonth.DateFrom.AddDays(daysAfterLock);

                    var isInLockPeriod = today <= lockDateLastDate;

                    var todayDateAtPreviousMonth = today.AddMonths(-2);
                    var todayDateAtThisMonth = today.AddMonths(-1);

                    var editingMonthDay = isInLockPeriod ? todayDateAtPreviousMonth : todayDateAtThisMonth;

                    var editingMonth = GetRangeOfMonthByDate(editingMonthDay);

                    return timeEntryDateEditing <= editingMonth.DateTo;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}