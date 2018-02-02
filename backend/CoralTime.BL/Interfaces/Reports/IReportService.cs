using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;
using System;

namespace CoralTime.BL.Interfaces.Reports
{
    public interface IReportService
    {
        DateTime DateFrom { get; set; }

        DateTime DateTo { get; set; }

        ReportsDropDownsView ReportsDropDowns();

        void SaveReportsSettings(RequestReportsSettings reportsSettings);

        ReportsGrandGridTimeEntryView ReportsGridGroupByNone(RequestReportsGrid reportsGridData);

        ReportsGrandGridProjectsView ReportsGridGroupByProjects(RequestReportsGrid reportsGridData);

        ReportsGrandGridMembersView ReportsGridGroupByUsers(RequestReportsGrid reportsGridData);

        ReportsGrandGridDatesView ReportsGridGroupByDates(RequestReportsGrid reportsGridData);

        ReportsGrandGridClients ReportsGridGroupByClients(RequestReportsGrid reportsGridData);
    }
}