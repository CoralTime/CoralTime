using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;
using System;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsService
    {
        DateTime DateFrom { get; }

        DateTime DateTo { get; }

        string SingleFilteredProjectName { get; }

        ReportDropDownsView ReportsDropDowns();

        ReportTotalView GetReportsGroupingBy(ReportsGridView reportsGridView);

        int SetGroupByOrDefaultGrouping(int? groupById);
    }
}