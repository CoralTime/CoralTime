using CoralTime.ViewModels.Reports.Request.Grid;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsSettingsService
    {
        ReportsSettingsView UpdateCurrentQuery(ReportsSettingsView reportsSettings, int memberImpersonatedId);

        void SaveCustomQuery(ReportsSettingsView reportsSettings);

        void DeleteCustomQuery(int id);
    }
}