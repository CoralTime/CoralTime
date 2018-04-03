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
            reportsSettingsView.GroupById = (int) Constants.ReportsGroupByIds.Date;
            reportsSettingsView.ShowColumnIds =
                new[]
                {
                    (int) Constants.ShowColumnModelIds.ShowEstimatedTime,
                    (int) Constants.ShowColumnModelIds.ShowDate,
                    (int) Constants.ShowColumnModelIds.ShowNotes,
                    (int) Constants.ShowColumnModelIds.ShowStartFinish
                };
            reportsSettingsView.DateStaticId = (int) Constants.DatesStaticIds.Today;

            return reportsSettingsView;
        }

        public static ReportsSettingsView GetView(this ReportsSettings reportsSettings)
        {
            var reportsSettingsView = new ReportsSettingsView();

            reportsSettingsView.GroupById = reportsSettings?.GroupById ?? (int) Constants.ReportsGroupByIds.Date;
            reportsSettingsView.ShowColumnIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterShowColumnIds);

            if (reportsSettings.DateFrom == null && reportsSettings.DateTo == null)
            {
                reportsSettingsView.DateStaticId = reportsSettings.DateStaticId;
            }
            else
            {
                reportsSettingsView.DateStaticId = null;
                reportsSettingsView.DateFrom = reportsSettings.DateFrom;
                reportsSettingsView.DateTo = reportsSettings.DateTo;
            }

            reportsSettingsView.ClientIds = CommonHelpers.ConvertStringToArrayOfNullableInts(reportsSettings.FilterClientIds);
            reportsSettingsView.ProjectIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterProjectIds);
            reportsSettingsView.MemberIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterMemberIds);

            reportsSettingsView.QueryName = reportsSettings.QueryName;
            reportsSettingsView.QueryId = reportsSettings.Id;

            return reportsSettingsView;
        }
    }
}
