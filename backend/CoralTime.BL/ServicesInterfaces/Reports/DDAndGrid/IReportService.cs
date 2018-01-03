using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.ReportsDropwDowns;
using CoralTime.ViewModels.Reports.Request;
using CoralTime.ViewModels.Reports.Request.ReportsGrid;
using System;
using System.Collections.Generic;

namespace CoralTime.BL.ServicesInterfaces.Reports.DDAndGrid
{
    // Get DropDowns and Grid. Filtration By / Grouping By: None, Projects, Users, Dates, Clients. 

    public interface IReportService
    {
        DateTime DateFrom { get; set; }

        DateTime DateTo { get; set; }

        ReportDropDownView GetReportsDropDowns(string userName);

        List<ReportsDropDownGroupBy> GetReportsDropDownGroupBy();

        ReportsGrandGridTimeEntryView GroupByNone(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridProjectsView GroupByProjects(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridMembersView GroupByUsers(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridDatesView GroupByDates(string userName, RequestReportsGrid reportsGridData);

        ReportsGrandGridClients GroupByClients(string userName, RequestReportsGrid reportsGridData);
    }
}