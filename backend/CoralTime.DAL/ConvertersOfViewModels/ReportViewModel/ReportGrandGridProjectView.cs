using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        public static ReportsGrandGridProjectsView GetViewReportsGrandGridClients(this ReportsGrandGridProjectsView reportsGridEntitiesProjects, Dictionary<Project, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridProjectView = timeEntries.Select(x => x.GetViewReportGridProject(_mapper));

            foreach (var groupingTimeEntries in reportGridProjectView)
            {
                reportsGridEntitiesProjects.GrandActualTime += groupingTimeEntries.TotalActualTime;
                reportsGridEntitiesProjects.GrandEstimatedTime += groupingTimeEntries.TotalEstimatedTime;
            }

            reportsGridEntitiesProjects.ReportsGridView = reportGridProjectView;

            return reportsGridEntitiesProjects;
        }

        private static ReportGridProjectView GetViewReportGridProject(this KeyValuePair<Project, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridProjectView = new ReportGridProjectView();

            SetReportsGridItemViewValues(timeEntries, reportGridProjectView, _mapper);

            reportGridProjectView.ProjectId = timeEntries.Key.Id;
            reportGridProjectView.ProjectName = timeEntries.Key.Name;

            return reportGridProjectView;
        }
    }
}