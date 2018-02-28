using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.DAL.ConvertersOfModels
{
    public static partial class ConvertersOfModels
    {
        private static void MapViewToModel(ReportsSettings reportsSettings, ReportsSettingsView reportsSettingsView, int memberId)
        {
            reportsSettings.QueryName = reportsSettingsView.QueryName;
            reportsSettings.MemberId = memberId;

            reportsSettings.GroupById = reportsSettingsView.GroupById ?? (int)Constants.ReportsGroupBy.Date;
            reportsSettings.DateFrom = reportsSettingsView.DateFrom;
            reportsSettings.DateTo = reportsSettingsView.DateTo;
            reportsSettings.FilterProjectIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ProjectIds);
            reportsSettings.FilterMemberIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.MemberIds);
            reportsSettings.FilterClientIds = CommonHelpers.ConvertFromArrayOfNullableIntsToString(reportsSettingsView.ClientIds);
            reportsSettings.FilterShowColumnIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ShowColumnIds);
        }
        
        public static ReportsSettings CreateModelForInsert(this ReportsSettings reportsSettings, ReportsSettingsView reportsSettingsView, int memberId)
        {
            reportsSettings = new ReportsSettings();

            MapViewToModel(reportsSettings, reportsSettingsView, memberId);

            return reportsSettings;
        }

        public static ReportsSettings UpdateModelForUpdates(this ReportsSettings reportsSettings, ReportsSettingsView reportsSettingsView, int memberId)
        {
            MapViewToModel(reportsSettings, reportsSettingsView, memberId);

            return reportsSettings;
        }
    }
}
