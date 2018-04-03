using CoralTime.ViewModels.DateFormat;
using CoralTime.ViewModels.Reports.Responce.DropDowns.GroupBy;
using System;
using System.Collections.Generic;

namespace CoralTime.Common.Constants
{
    public static class Constants
    {
        public const string UserTypeAdmins = "Admins";
        public const string UserTypeMembers = "Members";

        #region ApplicationRole

        public const string ApplicationRoleAdmin = "admin";
        public const string ApplicationRoleUser = "user";

        public static readonly IEnumerable<string> ApplicationRoles = new[] {ApplicationRoleAdmin, ApplicationRoleUser};

        #endregion

        #region ProjectRoles

        public const string ProjectRoleManager = "manager";
        public const string ProjectRoleMember = "team member";

        public static readonly IEnumerable<string> ProjectRoles = new[] {ProjectRoleManager, ProjectRoleMember};

        #endregion

        public const string JwtIsManagerClaimType = "isManager";
        public const string JwtRefreshTokenLifeTimeClaimType = "refreshTokenLifeTime";
        public const string ImpersonatedUserNameHeader = "Impersonate";

        public const string HeaderNameAuthorization = "Authorization";

        public const string SecureHeaderNameNotification = "SecureHeaderNameNotification";
        public const string SecureHeaderNameService = "SecureHeaderNameService";

        public const string SecureHeaderValueNotification = "SecureHeaderValueNotification";
        public const string SecureHeaderValueService = "SecureHeaderValueService";

        public const int SecondsInThisDay = 86400;

        public static string EnvName { get; set; }
        
        public const string CoralTime = "CoralTime";

        public enum LockTimePeriod
        {
            None = 0,
            Week = 1,
            Month = 2,
            Quarter = 3,
            Year = 4
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

            new DateConvert {DateFormatId = 6, DateFormat = "D/M/YY", DateFormatDotNet = "d/M/yy", DateFormatDotNetShort = "d/M"},
            new DateConvert {DateFormatId = 7, DateFormat = "D-M-YY", DateFormatDotNet = "d-M-yy", DateFormatDotNetShort = "d-M"},
            new DateConvert {DateFormatId = 8, DateFormat = "D.M.YY", DateFormatDotNet = "d.M.yy", DateFormatDotNetShort = "d.M"},

            new DateConvert {DateFormatId = 9, DateFormat = "M/D/YY", DateFormatDotNet = "M/d/yy", DateFormatDotNetShort = "M/d"},
            new DateConvert {DateFormatId = 10, DateFormat = "M-D-YY", DateFormatDotNet = "M-d-yy", DateFormatDotNetShort = "M-d"},
            new DateConvert {DateFormatId = 11, DateFormat = "M.D.YY", DateFormatDotNet = "M.d.yy", DateFormatDotNetShort = "M.d"},

            new DateConvert {DateFormatId = 12, DateFormat = "D/M/YYYY", DateFormatDotNet = "d/M/yyyy", DateFormatDotNetShort = "d/M"},
            new DateConvert {DateFormatId = 13, DateFormat = "D-M-YYYY", DateFormatDotNet = "d-M-yyyy", DateFormatDotNetShort = "d-M"},
            new DateConvert {DateFormatId = 14, DateFormat = "D.M.YYYY", DateFormatDotNet = "d.M.yyyy", DateFormatDotNetShort = "d.M"},
                                            
            new DateConvert {DateFormatId = 15, DateFormat = "M/D/YYYY", DateFormatDotNet = "M/d/yyyy", DateFormatDotNetShort = "M/d"},
            new DateConvert {DateFormatId = 16, DateFormat = "M-D-YYYY", DateFormatDotNet = "M-d-yyyy", DateFormatDotNetShort = "M-d"},
            new DateConvert {DateFormatId = 17, DateFormat = "M.D.YYYY", DateFormatDotNet = "M.d.yyyy", DateFormatDotNetShort = "M.d"},

        };

        public enum ReportsGroupByIds
        {
            Project = 1,
            Member = 2,
            Date = 3,
            Client = 4,
            UnknownGrouping = 5
        }

        public enum ShowColumnModelIds
        {
            ShowEstimatedTime = 1,
            ShowDate = 2,
            ShowNotes = 3,
            ShowStartFinish = 4
        }

        public enum DatesStaticIds
        {
            Today = 1,
            ThisWeek = 2,
            ThisMonth = 3,
            ThisYear = 4,

            Yesterday = 5,
            LastWeek = 6,
            LastMonth = 7,
            LastYear = 9
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

        #region Avatars and icons

        public static class Folders
        {
            public static string IconFolder => "Icons";

            public static string AvatarFolder => "Avatars";

            public static string StaticFilesFolder => "StaticFiles";
        }

        public const string ImageTypeAvatar = "ImageTypeAvatar";
        public const string ImageTypeIcon = "ImageTypeIcon";

        public const string ImageTypeSizeIcon = "40";
        public const string ImageTypeSizeAvatar = "200";

        #endregion
    }
}