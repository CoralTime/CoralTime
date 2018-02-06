using CoralTime.ViewModels.DateFormat;
using System;
using System.Collections.Generic;

namespace CoralTime.Common.Constants
{
    public static class Constants
    {
        public const string AdminRole = "admin";
        public const string ManagerRole = "manager";
        public const string UserRole = "user";
        public const string MemberRole = "team member";
        public const string JwtIsManagerClaimType = "isManager";
        public const string ImpersonatedUserNameHeader = "Impersonate";

        public const int SecondsInThisDay = 86400;


        public static string EnvName { get; set; }

        public const string CoralTime = "CoralTime";

        public const string SecureHeaderNotification = "SecureHeaderNotification";

        public const string SecureHeaderService = "SecureHeaderService";

        public enum LockTimePeriod
        {
            None = 0,
            Week = 1,
            Month = 2,
            Quarter = 3,
            Year = 4
        }

        public static IEnumerable<string> GetDefaultRoles()
        {
            return new[] {AdminRole, UserRole};
        }

        public static class WithoutClient
        {
            public const int Id = 0;
            public const string Name = "without client";
        }

        public enum WeekStart
        {
            Sunday = 0,
            Monday = 1
        }

        public enum Errors
        {
            None,
            EmailDoesntExist,
            ErrorSendEmail,
            InvalidToken,
            ErrorPassword,
            UserIsArchived
        }

        public static DateConvert[] DateFormats =
        {
            new DateConvert {DateFormatId = 0, DateFormat = "DD/MM/YYYY", DateFormatDotNet = "dd/MM/yyyy", DateFormatDotNetShort = "dd/MM"},
            new DateConvert {DateFormatId = 1, DateFormat = "DD-MM-YYYY", DateFormatDotNet = "dd-MM-yyyy", DateFormatDotNetShort = "dd-MM"},
            new DateConvert {DateFormatId = 2, DateFormat = "DD.MM.YYYY", DateFormatDotNet = "dd.MM.yyyy", DateFormatDotNetShort = "dd.MM"},
            new DateConvert {DateFormatId = 3, DateFormat = "MM/DD/YYYY", DateFormatDotNet = "MM/dd/yyyy", DateFormatDotNetShort = "MM/dd"},
            new DateConvert {DateFormatId = 4, DateFormat = "MM-DD-YYYY", DateFormatDotNet = "MM-dd-yyyy", DateFormatDotNetShort = "MM-dd"},
            new DateConvert {DateFormatId = 5, DateFormat = "MM.DD.YYYY", DateFormatDotNet = "MM.dd.yyyy", DateFormatDotNetShort = "MM.dd"},
        };

        public enum ReportsGroupBy
        {
            None = 0,
            Project = 1,
            User = 2,
            Date = 3,
            Client = 4,
            UnknownGrouping = 5
        }

        #region DayOfWeek (BitMask).

        [Flags]
        public enum DaysOfWeekForBinaryMask : short
        {
            Sunday = 1,
            Monday = 2,
            Tuesday = 4,
            Wednesday = 8,
            Thursday = 16,
            Friday = 32,
            Saturday = 64,
        }

        public class DaysOfWeekAdaptive
        {
            public short Id { get; set; }

            public short ValueForBinary { get; set; }

            public DayOfWeek DayOfWeek { get; set; }
        }

        public static readonly DaysOfWeekAdaptive[] daysOfWeekWithBinaryValues =
        {
            new DaysOfWeekAdaptive { Id = 0, ValueForBinary = (short) DaysOfWeekForBinaryMask.Sunday, DayOfWeek =  DayOfWeek.Sunday},
            new DaysOfWeekAdaptive { Id = 1, ValueForBinary = (short) DaysOfWeekForBinaryMask.Monday, DayOfWeek =  DayOfWeek.Monday},
            new DaysOfWeekAdaptive { Id = 2, ValueForBinary = (short) DaysOfWeekForBinaryMask.Tuesday, DayOfWeek =  DayOfWeek.Tuesday},
            new DaysOfWeekAdaptive { Id = 3, ValueForBinary = (short) DaysOfWeekForBinaryMask.Wednesday, DayOfWeek =  DayOfWeek.Wednesday},
            new DaysOfWeekAdaptive { Id = 4, ValueForBinary = (short) DaysOfWeekForBinaryMask.Thursday, DayOfWeek =  DayOfWeek.Thursday},
            new DaysOfWeekAdaptive { Id = 5, ValueForBinary = (short) DaysOfWeekForBinaryMask.Friday, DayOfWeek =  DayOfWeek.Friday},
            new DaysOfWeekAdaptive { Id = 6, ValueForBinary = (short) DaysOfWeekForBinaryMask.Saturday, DayOfWeek =  DayOfWeek.Saturday},
        };

        #endregion
    }
}