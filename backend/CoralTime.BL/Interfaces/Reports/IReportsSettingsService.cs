using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsSettingsService
    {
        void SaveCurrentQuery(ReportsSettingsView reportsSettings, string userName);

        void SaveCustomQuery(ReportsSettingsView reportsSettings, string userName);

        void DeleteCustomQuery(int id, string userName);
    }
}
