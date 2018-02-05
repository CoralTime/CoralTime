using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;
using System;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportsService
    {
        DateTime DateFrom { get; set; }

        DateTime DateTo { get; set; }

        ReportsDropDownsView ReportsDropDowns();

        ReportsGrandGridTimeEntryView ReportsGridGroupByNone(ReportsGridView reportsGridData);

        ReportsGrandGridProjectsView ReportsGridGroupByProjects(ReportsGridView reportsGridData);

        ReportsGrandGridMembersView ReportsGridGroupByUsers(ReportsGridView reportsGridData);

        ReportsGrandGridDatesView ReportsGridGroupByDates(ReportsGridView reportsGridData);

        ReportsGrandGridClients ReportsGridGroupByClients(ReportsGridView reportsGridData);
    }
}