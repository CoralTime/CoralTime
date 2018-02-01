using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ReportsSettings GetView(this ReportsSettingsView reportsSettingsView, ReportsSettings reportsSettings, int memberId)
        {
            return new ReportsSettings
            {
                Id = reportsSettings?.Id ?? 0,
                QueryName = reportsSettingsView.QueryName,
                MemberId = memberId,

                IsCurrentQuery = true,

                GroupById = reportsSettingsView.GroupById ?? (int)Constants.ReportsGroupBy.Date,
                DateFrom = reportsSettingsView.DateFrom,
                DateTo = reportsSettingsView.DateTo,
                FilterProjectIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ProjectIds),
                FilterMemberIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.MemberIds),
                FilterClientIds = CommonHelpers.ConvertFromArrayOfNullableIntsToString(reportsSettingsView.ClientIds),
                FilterShowColumnIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ShowColumnIds)
            };
        }
    }
}
