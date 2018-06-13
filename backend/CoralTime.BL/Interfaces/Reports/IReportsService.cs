using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Responce.DropDowns;
using CoralTime.ViewModels.Reports.Responce.Grid.ReportTotal;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsService
    {
        string SingleFilteredProjectName { get; }

        ReportDropDownView GetReportsDropDowns();

        ReportTotalView GetReportsGrid(ReportsGridView reportsGridView, Member memberFromNotification = null);

        void CheckAndSaveCurrentQuery(ReportsGridView reportsGridView);

        ReportTotalView InitializeReportTotalView(ReportsGridView reportsGridView);
    }
}