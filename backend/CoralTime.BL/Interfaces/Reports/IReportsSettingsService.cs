using CoralTime.ViewModels.Reports.Request.ReportsSettingsView;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsSettingsService
    {
        void UpdateCurrentQuery(ReportsSettingsView reportsSettings);

        void SaveCustomQuery(ReportsSettingsView reportsSettings);

        void DeleteCustomQuery(int id);
    }
}