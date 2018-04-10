using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ReportsSettingsView GetViewWithDefaultValues(this ReportsSettingsView reportsSettingsView)
        {
            reportsSettingsView.DateStaticId = (int)Constants.DatesStaticIds.ThisWeek;
            reportsSettingsView.GroupById = (int) Constants.ReportsGroupByIds.Date;
            reportsSettingsView.ShowColumnIds =
                new[]
                {
                    (int) Constants.ShowColumnModelIds.ShowEstimatedTime,
                    (int) Constants.ShowColumnModelIds.ShowDate,
                    (int) Constants.ShowColumnModelIds.ShowNotes,
                    (int) Constants.ShowColumnModelIds.ShowStartFinish
                };

            return reportsSettingsView;
        }

        public static ReportsSettingsView GetView(this ReportsSettings reportsSettings)
        {
            return new ReportsSettingsView
            {
                GroupById = reportsSettings?.GroupById,
                ShowColumnIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterShowColumnIds),

                DateStaticId = reportsSettings.DateStaticId,
                DateFrom = reportsSettings.DateFrom,
                DateTo = reportsSettings.DateTo,

                ClientIds = CommonHelpers.ConvertStringToArrayOfNullableInts(reportsSettings.FilterClientIds),
                ProjectIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterProjectIds),
                MemberIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterMemberIds),

                QueryName = reportsSettings.QueryName,
                QueryId = reportsSettings.Id,
            };
        }
    }
}
