using System;
using DatesStaticIds = CoralTime.Common.Constants.Constants.DatesStaticIds;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfThisWeek(DateTime today, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return GetRangeOfWeekByDate(today, startOfWeek);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfThisMonth(DateTime today)
        {
            return GetRangeOfMonthByDate(today);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfThisYear(DateTime today) 
        {
            return GetRangeOfYearByDate(today);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastWeek(DateTime today, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var thisWeek = GetRangeOfThisWeek(today, startOfWeek);
            return (thisWeek.DateFrom.AddDays(-7).Date, thisWeek.DateTo.AddDays(-7).Date);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastMonth(DateTime today)
        {
            return GetRangeOfMonthByDate(today.AddMonths(-1));
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastYear(DateTime today)
        {
            return GetRangeOfYearByDate(today.AddYears(-1));
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfWeekByDate(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = date.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            
            var weekDateStart = date.AddDays(-1 * diff).Date;
            var weekDateEnd = weekDateStart.AddDays(7).AddMilliseconds(-1).Date;
            return (weekDateStart, weekDateEnd);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfMonthByDate(DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddMilliseconds(-1).Date;
            return (firstDay, lastDay);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfQuarterByDate(DateTime date)
        {
            var quarterId = (date.Month -1) / 3;
            var firstDay = new DateTime(date.Year, quarterId * 3 + 1, 1);
            var lastDay = firstDay.AddMonths(3).AddMilliseconds(-1).Date;
            return (firstDay, lastDay);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastQuarterByDate(DateTime date)
        {
            return GetRangeOfQuarterByDate(date.AddMonths(-3));
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfYearByDate(DateTime date)
        {
            var firstDay = new DateTime(date.Year, 1, 1);
            var lastDay = firstDay.AddYears(1).AddMilliseconds(-1).Date;
            return (firstDay, lastDay);
        }
        
        public static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastWorkWeekByDate(DateTime today, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var lastWeek = GetRangeOfLastWeek(today, startOfWeek);
            return (lastWeek.DateFrom, lastWeek.DateTo.AddDays(-2).Date);
        }

        public static (DateTime DateFrom, DateTime DateTo) GetPeriod(int dayStaticId, DateTime? todayDate, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return GetPeriod((DatesStaticIds) dayStaticId, todayDate, startOfWeek);
        }

        public static (DateTime DateFrom, DateTime DateTo) GetPeriod(DatesStaticIds dayStaticId, DateTime? todayDate, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var date = (todayDate == null)? DateTime.Today : todayDate.Value.Date;
            var today = new DateTime(date.Year, date.Month, date.Day);
            var yesterday = today.AddDays(-1).Date;
            switch (dayStaticId)
            {
                case DatesStaticIds.Today:
                    return (today, today);

                case DatesStaticIds.Yesterday:
                    return (yesterday, yesterday);

                case DatesStaticIds.ThisWeek:
                    return GetRangeOfThisWeek(today, startOfWeek);

                case DatesStaticIds.ThisMonth:
                    return GetRangeOfThisMonth(today);

                case DatesStaticIds.ThisYear:
                    return GetRangeOfThisYear(today);

                case DatesStaticIds.LastWeek:
                    return GetRangeOfLastWeek(today, startOfWeek);

                case DatesStaticIds.LastMonth:
                    return GetRangeOfLastMonth(today);

                case DatesStaticIds.LastYear:
                    return GetRangeOfLastYear(today);

                case DatesStaticIds.ThisQuarter:
                    return GetRangeOfQuarterByDate(today);

                case DatesStaticIds.LastQuarter:
                    return GetRangeOfLastQuarterByDate(today);

                default:
                    return (today, today);
            }
        }
    }
}