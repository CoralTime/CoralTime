using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ReportsSettings GetView(this ReportsSettingsView reportsSettingsView, ReportsSettings reportsSettingsFromDb, bool isDefaultQuery, string queryName, int memberId)
        {
            return new ReportsSettings
            {
                Id = reportsSettingsFromDb?.Id ?? 0,
                IsDefaultQuery = isDefaultQuery,
                QueryName = queryName,
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
    }
}
