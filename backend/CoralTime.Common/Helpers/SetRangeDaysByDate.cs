using System;
using CoralTime.Common.Constants;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        public static void SetRangeOfThisWeekByDate(out DateTime weekDateStart, out DateTime weekDateEnd, DateTime dayOfWeek, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = dayOfWeek.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            weekDateStart = dayOfWeek.AddDays(-1 * diff).Date;
            weekDateEnd = weekDateStart.AddDays(7).AddMilliseconds(-1);
        }

        public static void SetRangeOfThisMonthByDate(out DateTime monthDateStart, out DateTime monthDateEnd, DateTime day)
        {
            monthDateStart = new DateTime(day.Year, day.Month, 1);
            monthDateEnd = new DateTime(day.Year, day.Month, 1).AddMonths(1).AddMilliseconds(-1);
        }

        public static void SetRangeOfThisYearByDate(out DateTime yearDateStart, out DateTime yearDateEnd, DateTime day)
        {
            yearDateStart = new DateTime(day.Year, 1, 1);
            yearDateEnd = new DateTime(day.Year + 1, 1, 1).AddMilliseconds(-1);
        }

        public static void SetRangeOfLastWeekByDate(out DateTime lastWeekStart, out DateTime lastWeekEnd, DateTime dayOfWeek, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            SetRangeOfThisWeekByDate(out lastWeekStart, out lastWeekEnd, dayOfWeek, startOfWeek);

            lastWeekStart = lastWeekStart.AddDays(-7);
            lastWeekEnd = lastWeekEnd.AddDays(-7);
        }

        public static void SetRangeOfLastMonthByDate(out DateTime lastMonthDateStart, out DateTime lastMonthDateEnd, DateTime day)
        {
            SetRangeOfThisMonthByDate(out lastMonthDateStart, out lastMonthDateEnd, day);

            lastMonthDateStart = lastMonthDateStart.AddMonths(-1);
            lastMonthDateEnd = lastMonthDateEnd.AddMonths(-1);
        }

        public static void SetRangeOfLastYearByDate(out DateTime lastYearDateStart, out DateTime lastYearDateEnd, DateTime day)
        {
            SetRangeOfThisYearByDate(out lastYearDateStart, out lastYearDateEnd, day);

            lastYearDateStart = lastYearDateStart.AddYears(-1);
            lastYearDateEnd = lastYearDateEnd.AddYears(-1);
        }

        public static void SetRangeOfWorkWeekByDate(out DateTime workWeekDateStart, out DateTime workWeekDateEnd, DateTime dayOfWeek)
        {
            SetRangeOfThisWeekByDate(out workWeekDateStart, out workWeekDateEnd, dayOfWeek);

            workWeekDateEnd = workWeekDateEnd.AddDays(-2);
        }

        public static void SetRangeOfLastWorkWeekByDate(out DateTime lastWorkWeekStart, out DateTime lastWorkWeekEnd, DateTime dayOfWeek, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            SetRangeOfLastWeekByDate(out lastWorkWeekStart, out lastWorkWeekEnd, dayOfWeek, startOfWeek);

            lastWorkWeekEnd = lastWorkWeekEnd.AddDays(-2);
        }

        public static (DateTime DateFrom, DateTime DateTo) GetPeriod(int? dayStaticId, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var today = DateTime.Today;
            switch (dayStaticId)
            {
                case (int) Constants.Constants.DatesStaticIds.Today:
                    return (today, today);
                        
                case (int) Constants.Constants.DatesStaticIds.Yesterday:
                    return (today.AddDays(-1), today.AddDays(-1));
                
                case (int) Constants.Constants.DatesStaticIds.ThisWeek:
                    SetRangeOfThisWeekByDate(out var thisWeekStart, out var thisWeekEnd, today, startOfWeek);
                    return (thisWeekStart, thisWeekEnd);
  
                case (int) Constants.Constants.DatesStaticIds.ThisMonth:
                    SetRangeOfThisMonthByDate(out var monthDateStart, out var monthDateEnd, today);
                    return (monthDateStart, monthDateEnd);
                
                case (int) Constants.Constants.DatesStaticIds.ThisYear:
                    SetRangeOfThisYearByDate(out var yearDateStart, out var yearDateEnd, today);
                    return (yearDateStart, yearDateEnd);
                
                case (int) Constants.Constants.DatesStaticIds.LastWeek:
                    SetRangeOfLastWeekByDate(out var lastWeekStart, out var lastWeekEnd, today, startOfWeek);
                    return (lastWeekStart, lastWeekEnd);    
                
                case (int) Constants.Constants.DatesStaticIds.LastMonth:
                    SetRangeOfLastMonthByDate(out var lastMonthDateStart, out var lastMonthDateEnd, today);
                    return (lastMonthDateStart, lastMonthDateEnd);  

                case (int) Constants.Constants.DatesStaticIds.LastYear:
                    SetRangeOfLastYearByDate(out var lastYearDateStart, out var lastYearDateEnd, today);
                    return (lastYearDateStart, lastYearDateEnd);  
                
                    default:
                        return (today, today);
            }
        }
    }
}