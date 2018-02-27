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

        ReportsDropDownsView ReportsDropDowns();

        ReportsTotalGridTimeEntryView ReportsGridGroupByNone(ReportsGridView reportsGridData);

        ReportsTotalGridProjectsView GetGroupingReportsGridByProjects(ReportsGridView reportsGridData);

        ReportsTotalGridMembersView GetGroupingReportsGridByUsers(ReportsGridView reportsGridData);

        ReportsTotalGridByDatesView GetGroupingReportsGridByDates(ReportsGridView reportsGridData);

        ReportsTotalGridClients GetGroupingReportsGridByClients(ReportsGridView reportsGridData);
    }
}