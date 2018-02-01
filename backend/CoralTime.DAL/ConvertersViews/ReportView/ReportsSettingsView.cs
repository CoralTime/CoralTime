using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ReportsSettings GetViewInsert(this ReportsSettingsView reportsSettingsView, ReportsSettings reportsSettingsFromDb, int memberId)
        {
            return new ReportsSettings
            {
                Id = reportsSettingsFromDb?.Id ?? 0,
                QueryName = reportsSettingsView.QueryName,
                MemberId = memberId,

                GroupById = reportsSettingsView.GroupById ?? (int)Constants.ReportsGroupBy.Date,
                DateFrom = reportsSettingsView.DateFrom,
                DateTo = reportsSettingsView.DateTo,
                FilterProjectIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ProjectIds),
                FilterMemberIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.MemberIds),
                FilterClientIds = CommonHelpers.ConvertFromArrayOfNullableIntsToString(reportsSettingsView.ClientIds),
                FilterShowColumnIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ShowColumnIds)
            };
        }

        public static ReportsSettings GetViewUpdate(this ReportsSettingsView reportsSettingsView, ReportsSettings reportsSettings, int memberId)
        {
            reportsSettings.Id = reportsSettings?.Id ?? 0;
            reportsSettings.QueryName = reportsSettingsView.QueryName;
            reportsSettings.MemberId = memberId;

            reportsSettings.GroupById = reportsSettingsView.GroupById ?? (int)Constants.ReportsGroupBy.Date;
            reportsSettings.DateFrom = reportsSettingsView.DateFrom;
            reportsSettings.DateTo = reportsSettingsView.DateTo;
            reportsSettings.FilterProjectIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ProjectIds);
            reportsSettings.FilterMemberIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.MemberIds);
            reportsSettings.FilterClientIds = CommonHelpers.ConvertFromArrayOfNullableIntsToString(reportsSettingsView.ClientIds);
            reportsSettings.FilterShowColumnIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ShowColumnIds);

            return reportsSettings;
        }
    }
}
