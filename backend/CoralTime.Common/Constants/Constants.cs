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

        #region Url adresses

        public const string UrlSetPassword = "/set-password";

        public static readonly string[] AngularRoutes =
        {
            "/home",
            "/profile",
            "/projects",
            "/clients",
            "/about",
            "/login",
            "/tasks",
            "/users",
            "/reports",
            "/calendar",
            "/settings",
            "/help",
            "/signin-oidc",
            UrlSetPassword
        };

        public static class Routes
        {
            public const string BaseControllerRoute = "api/v1/[controller]";
            public const string IdRoute = "{id}";
            public const string IdRouteWithMembers = IdRoute + WithMembers;
            public const string IdRouteWithProjects = IdRoute + WithProjects;
            public const string UpdateManagerRolesRoute = "UpdateManagerRoles";
            public const string UpdateClaimsRoute = "UpdateClaims";
            public const string RefreshDataBaseRoute = "RefreshDataBase";
            public const string SaveImagesFromDbToStaticFilesRoute = "SaveImagesFromDbToStaticFiles";
            public const string SendForgotEmailRoute = "sendforgotemail/{email}";
            public const string ChangePasswordByTokenRoute = "changepasswordbytoken";
            public const string ChangePasswordByTokenWithTokenRoute = ChangePasswordByTokenRoute + WithToken;
            public const string CheckPasswordByTokenRoute = "checkforgotpasswordtoken";
            public const string CheckPasswordByTokenWithTokenRoute = CheckPasswordByTokenRoute + WithToken;
            public const string MemberRoute = "Member(" + IdRoute + ")";
            public const string MemberRouteWithNotifications = MemberRoute + WithNotifications;
            public const string MemberRouteWithPreferences = MemberRoute + WithPreferences;
            public const string MemberRouteWithPersonalInfo = MemberRoute + WithPersonalInfo;
            public const string MemberRouteWithUrlAvatar = MemberRoute + WithUrlAvatar;
            public const string UploadImageRoute = "UploadImage";
            public const string ProjectsRoute = "Projects";
            public const string DateFormatsRoute = "DateFormats";
            public const string ProjectMembersWithIdRoute = "ProjectMembers/" + IdRoute;
            public const string ByProjectSettingsRoute = "ByProjectSettings";
            public const string SendWeeklyTimeEntryUpdatesRoute = "SendWeeklyTimeEntryUpdates";
            public const string CustomQueryRoute = "CustomQuery";
            public const string CustomQueryWithIdRoute = CustomQueryRoute + "/" + IdRoute;
            public const string AuthorizeRoute = "authorize";
            public const string AuthorizeActiveUserRoute = AuthorizeRoute + "/activeuser";
            public const string AuthorizeAdminRoute = AuthorizeRoute + "/admin";
            public const string AuthorizeUserRoute = AuthorizeRoute + "/user";
            public const string UnauthorizeRoute = "unauthorize";
            public const string PingRoute = "ping";
            public const string PingdatabaseRoute = "pingdatabase";
            
            private const string WithMembers = "/members";
            private const string WithProjects = "/projects";
            private const string WithNotifications = "/Notifications";
            private const string WithPreferences = "/Preferences";
            private const string WithPersonalInfo = "/PersonalInfo";
            private const string WithUrlAvatar = "/UrlAvatar";
            private const string WithToken = "/{token}";
            
            public static class OData
            {
                public const string BaseODataRoute = "api/v1/odata";
                public const string BaseODataControllerRoute = BaseODataRoute + "/[controller]";
                public const string TasksWithIdRoute = "Tasks(" + IdRoute + ")";
                public const string ClientsWithIdRoute = "Clients(" + IdRoute + ")";
                public const string ProjectsWithIdRoute = "Projects(" + IdRoute + ")";
                public const string ProjectsRouteWithMembers = ProjectsWithIdRoute + WithMembers;
                public const string MembersWithIdRoute = "Members(" + IdRoute + ")";
                public const string MembersRouteWithProjects = MembersWithIdRoute + WithProjects;
                public const string MemberProjectRolesWithIdRoute = "MemberProjectRoles(" + IdRoute + ")";
                public const string MemberProjectRolesRouteWithProjects = MemberProjectRolesWithIdRoute + WithProjects;
                public const string MemberProjectRolesRouteWithMembers = MemberProjectRolesWithIdRoute + WithMembers;
            }
        }

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

        public enum FileType
        {
            Excel = 0,
            CSV = 1,
            PDF = 2
        }

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
            User = 2,
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