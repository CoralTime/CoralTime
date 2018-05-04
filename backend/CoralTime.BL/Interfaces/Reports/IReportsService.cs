using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsService
    {
        string SingleFilteredProjectName { get; }

        ReportDropDownView GetReportsDropDowns();

        ReportTotalView GetReportsGrid(ReportsGridView reportsGridView);

        void CheckAndSaveCurrentQuery(ReportsGridView reportsGridView);
    }
}