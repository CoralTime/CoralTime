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

        ReportsDropDownsView ReportsDropDowns(string userName);

        ReportsGrandGridTimeEntryView ReportsGridGroupByNone(string userName, ReportsGridView reportsGridData);

        ReportsGrandGridProjectsView ReportsGridGroupByProjects(string userName, ReportsGridView reportsGridData);

        ReportsGrandGridMembersView ReportsGridGroupByUsers(string userName, ReportsGridView reportsGridData);

        ReportsGrandGridDatesView ReportsGridGroupByDates(string userName, ReportsGridView reportsGridData);

        ReportsGrandGridClients ReportsGridGroupByClients(string userName, ReportsGridView reportsGridData);
    }
}