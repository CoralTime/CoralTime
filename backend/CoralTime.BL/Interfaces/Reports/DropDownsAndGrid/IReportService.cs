using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.ReportsDropwDowns;
using CoralTime.ViewModels.Reports.Request;
using CoralTime.ViewModels.Reports.Request.Grid;
using System;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces.Reports.DropDownsAndGrid
{
    // Get DropDowns and Grid. Filtration By / Grouping By: None, Projects, Users, Dates, Clients. 

    public interface IReportService
    {
        DateTime DateFrom { get; set; }

        DateTime DateTo { get; set; }

        ReportDropDownView ReportsDropDowns(string userName);

        List<ReportsDropDownGroupBy> ReportsDropDownGroupBy();

        void SaveReportsSettings(IRequestReportsSettings reportsSettings, string userName);

        ReportsGrandGridTimeEntryView ReportsGridGroupByNone(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridProjectsView ReportsGridGroupByProjects(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridMembersView ReportsGridGroupByUsers(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridDatesView ReportsGridGroupByDates(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridClients ReportsGridGroupByClients(string userName, RequestReportsGrid reportsGridData);
    }
}