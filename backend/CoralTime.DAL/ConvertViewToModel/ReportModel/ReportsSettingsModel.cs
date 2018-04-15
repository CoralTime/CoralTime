using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.DAL.ConvertViewToModel
{
    public static partial class ConvertViewToModel
    {
        public static ReportsSettings GetModel(this ReportsSettings reportsSettings, ReportsSettingsView reportsSettingsView, int memberId)
        {
            reportsSettings.QueryName = reportsSettingsView.QueryName;
            reportsSettings.MemberId = memberId;

            reportsSettings.IsCurrentQuery = true;

            reportsSettings.GroupById = reportsSettingsView.GroupById;
            reportsSettings.DateFrom = reportsSettingsView.DateFrom;
            reportsSettings.DateTo = reportsSettingsView.DateTo;
            reportsSettings.DateStaticId = reportsSettingsView.DateStaticId;

            reportsSettings.FilterProjectIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ProjectIds);
            reportsSettings.FilterMemberIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.MemberIds);
            reportsSettings.FilterClientIds = CommonHelpers.ConvertFromArrayOfNullableIntsToString(reportsSettingsView.ClientIds);
            reportsSettings.FilterShowColumnIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ShowColumnIds);

            return reportsSettings;
        }
    }
}
