using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ReportsTotalGridProjectsView GetViewReportsTotalGridProjects(this ReportsTotalGridProjectsView reportsGridEntitiesProjects, Dictionary<Project, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridProjectView = timeEntries.Select(x => x.GetViewReportGridProject(_mapper));

            foreach (var groupingTimeEntries in reportGridProjectView)
            {
                reportsGridEntitiesProjects.TotalActualTime += groupingTimeEntries.TotalForActualTime;
                reportsGridEntitiesProjects.TotalEstimatedTime += groupingTimeEntries.TotalForEstimatedTime;
            }

            reportsGridEntitiesProjects.ReportsGridView = reportGridProjectView;

            return reportsGridEntitiesProjects;
        }

        private static ReportTotalForGridProjectView GetViewReportGridProject(this KeyValuePair<Project, IEnumerable<TimeEntry>> timeEntries, IMapper _mapper)
        {
            var reportGridProjectView = new ReportTotalForGridProjectView();

            SetReportsExportTotalForAndItemsValues(timeEntries, reportGridProjectView, _mapper);

            reportGridProjectView.ProjectId = timeEntries.Key.Id;
            reportGridProjectView.ProjectName = timeEntries.Key.Name;

            return reportGridProjectView;
        }
    }
}