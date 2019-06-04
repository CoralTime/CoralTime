using System;
using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models.ReportsSettings;
using CoralTime.ViewModels.Reports.Request.ReportsSettingsView;

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

        public static ReportsSettingsView GetView(this ReportsSettings reportsSettings, DayOfWeek startOfWeek, DateTime? today)
        {
            var settings = new ReportsSettingsView
            {
                GroupById = reportsSettings?.GroupById,
                ShowColumnIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterShowColumnIds),

                DateStaticId = reportsSettings.DateStaticId,
                IsTotalsOnly = reportsSettings.IsTotalsOnly,

                ClientIds = CommonHelpers.ConvertStringToArrayOfNullableInts(reportsSettings.FilterClientIds),
                ProjectIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterProjectIds),
                MemberIds = CommonHelpers.ConvertStringToArrayOfInts(reportsSettings.FilterMemberIds),

                QueryName = reportsSettings.QueryName,
                QueryId = reportsSettings.Id,
            };

            if (settings.DateStaticId == null)
            {
                settings.DateFrom = reportsSettings.DateFrom;
                settings.DateTo = reportsSettings.DateTo;
            }
            else
            {
                var period = CommonHelpers.GetPeriod(settings.DateStaticId ?? 1, today, startOfWeek);
                settings.DateFrom = period.DateFrom;
                settings.DateTo = period.DateTo;
            }

            return settings;
        }
    }
}
