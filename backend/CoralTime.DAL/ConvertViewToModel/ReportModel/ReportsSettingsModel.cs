using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.DAL.ConvertViewToModel
{
    public static partial class ConvertViewToModel
    {
        private static void MapViewToModel(ReportsSettings reportsSettings, ReportsSettingsView reportsSettingsView, int memberId)
        {
            reportsSettings.QueryName = reportsSettingsView.QueryName;
            reportsSettings.MemberId = memberId;

            reportsSettings.IsCurrentQuery = true;

            reportsSettings.GroupById = reportsSettingsView.GroupById ?? (int) Constants.ReportsGroupByIds.Date;
            reportsSettings.DateFrom = reportsSettingsView.DateFrom;
            reportsSettings.DateTo = reportsSettingsView.DateTo;
            reportsSettings.FilterProjectIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ProjectIds);
            reportsSettings.FilterMemberIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.MemberIds);
            reportsSettings.FilterClientIds = CommonHelpers.ConvertFromArrayOfNullableIntsToString(reportsSettingsView.ClientIds);
            reportsSettings.FilterShowColumnIds = CommonHelpers.ConvertFromArrayOfIntsToString(reportsSettingsView.ShowColumnIds);
        }
        
        public static ReportsSettings GetModel(this ReportsSettings reportsSettings, ReportsSettingsView reportsSettingsView, int memberId)
        {
            MapViewToModel(reportsSettings, reportsSettingsView, memberId);

            if (reportsSettingsView.DateFrom == null && reportsSettingsView.DateTo == null && reportsSettingsView.DateStaticId == null)
            {
                reportsSettings.DateStaticId = (int) Constants.DatesStaticIds.Today;
            }
            else
            {
                reportsSettings.DateFrom = null;
                reportsSettings.DateTo = null;
                
                reportsSettings.DateStaticId = reportsSettingsView.DateStaticId ?? (int)Constants.DatesStaticIds.Today;
            }

            return reportsSettings;
        }
    }
}
