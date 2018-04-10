using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsSettingsService
    {
        ReportsSettingsView GetCurrentOrCreateDefaultQuery();

        ReportsSettingsView SaveCurrentQuery(ReportsSettingsView reportsSettings);

        void SaveCustomQuery(ReportsSettingsView reportsSettings);

        void DeleteCustomQuery(int id);
    }
}