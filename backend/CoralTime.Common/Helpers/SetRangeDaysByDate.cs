using System;
using DatesStaticIds = CoralTime.Common.Constants.Constants.DatesStaticIds;

namespace CoralTime.Common.Helpers
{
    public partial class CommonHelpers
    {
        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfThisWeek(DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return GetRangeOfWeekByDate(DateTime.Today, startOfWeek);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfThisMonth()
        {
            return GetRangeOfMonthByDate(DateTime.Today);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfThisYear() 
        {
            return GetRangeOfYearByDate(DateTime.Today);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastWeek(DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var thisWeek = GetRangeOfThisWeek(startOfWeek);
            return (thisWeek.DateFrom.AddDays(-7).Date, thisWeek.DateTo.AddDays(-7).Date);
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastMonth()
        {
            return GetRangeOfMonthByDate(DateTime.Today.AddMonths(-1));
        }

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastYear()
        {
            return GetRangeOfYearByDate(DateTime.Today.AddYears(-1));
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

        private static (DateTime DateFrom, DateTime DateTo) GetRangeOfYearByDate(DateTime date)
        {
            var firstDay = new DateTime(date.Year, 1, 1);
            var lastDay = firstDay.AddYears(1).AddMilliseconds(-1).Date;
            return (firstDay, lastDay);
        }
        
        public static (DateTime DateFrom, DateTime DateTo) GetRangeOfLastWorkWeekByDate(DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var lastWeek = GetRangeOfLastWeek(startOfWeek);
            return (lastWeek.DateFrom, lastWeek.DateTo.AddDays(-2).Date);
        }

        public static (DateTime DateFrom, DateTime DateTo) GetPeriod(int dayStaticId, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return GetPeriod((DatesStaticIds) dayStaticId, startOfWeek);
        }

        public static (DateTime DateFrom, DateTime DateTo) GetPeriod(DatesStaticIds dayStaticId, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var today = DateTime.Today.Date;
            var yesterday = DateTime.Today.AddDays(-1).Date;
            switch (dayStaticId)
            {
                case DatesStaticIds.Today:
                    return (today, today);
                        
                case DatesStaticIds.Yesterday:
                    return (yesterday, yesterday);
                
                case DatesStaticIds.ThisWeek:
                    return GetRangeOfThisWeek(startOfWeek);
  
                case DatesStaticIds.ThisMonth:
                    return GetRangeOfThisMonth();
                
                case DatesStaticIds.ThisYear:
                    return GetRangeOfThisYear();
                
                case DatesStaticIds.LastWeek:
                    return GetRangeOfLastWeek(startOfWeek);
                
                case DatesStaticIds.LastMonth:
                    return GetRangeOfLastMonth();  

                case DatesStaticIds.LastYear:
                    return GetRangeOfLastYear();  
                
                default:
                    return (today, today);
            }
        }
    }
}