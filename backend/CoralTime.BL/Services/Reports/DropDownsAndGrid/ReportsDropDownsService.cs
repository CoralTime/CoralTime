using CoralTime.BL.Services.Reports.Export;
using CoralTime.Common.Constants;
using CoralTime.DAL.ConvertersViews.ExstensionsMethods;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Request.Grid;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;
using CoralTime.ViewModels.Reports.Responce.DropDowns.GroupBy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportService
    {
        private readonly List<ReportsDropDownGroupBy> _dropDownGroupBy = new List<ReportsDropDownGroupBy>
        {
            new ReportsDropDownGroupBy
            {
                Id = (int) Constants.ReportsGroupBy.Project,
                Description = Constants.ReportsGroupBy.Project.ToString()
            },

            new ReportsDropDownGroupBy
            {
                Id = (int) Constants.ReportsGroupBy.User,
                Description = Constants.ReportsGroupBy.User.ToString()
            },

            new ReportsDropDownGroupBy
            {
                Id = (int) Constants.ReportsGroupBy.Date,
                Description = Constants.ReportsGroupBy.Date.ToString()
            },

            new ReportsDropDownGroupBy
            {
                Id = (int) Constants.ReportsGroupBy.Client,
                Description = Constants.ReportsGroupBy.Client.ToString()
            }
        };

        public ReportsDropDownsView ReportsDropDowns(string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
            var memberByUserName = Uow.MemberRepository.LinkedCacheGetByName(userName);

            var reportDropDowns = new ReportsDropDownsView
            {
                Values = CreateDropDownValues(memberByUserName),
                CurrentQuery = CreateDropDownValuesSaved(memberByUserName.Id)
            };

            return reportDropDowns;
        }

        private ReportsDropDownValues CreateDropDownValues(Member member)
        {
            var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();
            var memberRoleId = Uow.ProjectRoleRepository.GetMemberRoleId();

            var projectsOfClients = new List<Project>();
            var clientsFromProjectOfClients = new List<Client>();
            var members = Uow.MemberRepository.LinkedCacheGetList();

            #region GetProjects allProjectsForAdmin or projectsWithAssignUsersAndPublicProjects.

            if (member.User.IsAdmin)
            {
                var allProjectsForAdmin = Uow.ProjectRepository.LinkedCacheGetList().ToList();
                projectsOfClients = allProjectsForAdmin;
            }
            else
            {
                var projectsWithAssignUsersAndPublicProjects = Uow.ProjectRepository.LinkedCacheGetList()
                    .Where(x => x.MemberProjectRoles.Select(z => z.MemberId).Contains(member.Id) || !x.IsPrivate).ToList();

                projectsOfClients.AddRange(projectsWithAssignUsersAndPublicProjects);
            }

            #endregion

            #region Get Clients from Projects of clients.

            // 1. Get all clients from targeted projects where project is assign to client.
            var clientsWithProjects = projectsOfClients.Where(x => x.Client != null).Select(x => x.Client).Distinct().ToList();

            foreach (var client in clientsWithProjects)
            {
                client.Projects = client.Projects.Where(proj => projectsOfClients.Select(pc => pc.Id).Contains(proj.Id)).ToList();
            }

            clientsFromProjectOfClients.AddRange(clientsWithProjects);

            // 2. Get all projects where project is not assign to client and create client "WithoutClients" that we add projects to it.
            var hasClientsWithoutProjects = projectsOfClients.Where(x => x.Client == null).Any();
            if (hasClientsWithoutProjects)
            {
                var clientWithoutProjects = new Client
                {
                    Id = Constants.WithoutClient.Id,
                    Name = Constants.WithoutClient.Name,
                    IsActive = true,
                    Projects = projectsOfClients.Where(x => x.Client == null).ToList()
                };

                clientsFromProjectOfClients.Add(clientWithoutProjects);
            }

            #endregion

            var reportClientView = new List<ReportClientView>();

            foreach (var client in clientsFromProjectOfClients)
            {
                var reportProjectViewByUserId = new List<ReportProjectView>();

                foreach (var project in client.Projects)
                {
                    var reportProjectView = new ReportProjectView
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        RoleId = project.MemberProjectRoles.FirstOrDefault(r => r.MemberId == member.Id)?.RoleId ?? 0,
                        IsProjectActive = project.IsActive,
                    };

                    #region Set all users at Project constrain only for: Admin, Manager at this project.

                    var isManagerOnProject = project.MemberProjectRoles.Exists(r => r.MemberId == member.Id && r.RoleId == managerRoleId);

                    if (member.User.IsAdmin || isManagerOnProject)
                    {
                        var usersDetailsView = project.MemberProjectRoles.Select(x => x.Member.GetViewReportUsers(x.RoleId, Mapper)).ToList();

                        // Add members, that is not assigned to this project directly.
                        if (!project.IsPrivate)
                        {
                            var notAssignedMembersAtProjView = members.Where(x => project.MemberProjectRoles.Select(y => y.MemberId).All(mi => x.Id != mi)).Select(u => u.GetViewReportUsers(memberRoleId, Mapper));
                            usersDetailsView.AddRange(notAssignedMembersAtProjView);
                        }

                        // Set all users of the project.
                        reportProjectView.UsersDetails = usersDetailsView;
                    }

                    #endregion

                    reportProjectView.IsUserManagerOnProject = isManagerOnProject;
                    reportProjectViewByUserId.Add(reportProjectView);
                }

                var reportClientViewlocal = new ReportClientView
                {
                    ClientId = client.Id,
                    ClientName = client.Name,
                    IsClientActive = client.IsActive,
                    ProjectsDetails = reportProjectViewByUserId
                };

                reportClientView.Add(reportClientViewlocal);
            }

            var userDetails = new ReportsUserDetails
            {
                CurrentUserFullName = member.FullName,
                CurrentUserId = member.Id,
                IsAdminCurrentUser = member.User.IsAdmin,
                IsManagerCurrentUser = member.User.IsManager,
            };

            var valuesCustomQueries = new List<ReportsSettingsView>();

            var customQueries = Uow.ReportsSettingsRepository.GetQueriesByMemberIdWithIncludes(member.Id).Where(x => !x.IsCurrentQuery);
            foreach (var customReportSettings in customQueries)
            {
                valuesCustomQueries.Add(CreateReportsSettingsEntity(customReportSettings, false));
            }

            var dropDownValues = new ReportsDropDownValues
            {
                Filters = reportClientView,
                GroupBy = _dropDownGroupBy,
                ShowColumns = ReportsExportService.showColumnsInfo,
                UserDetails = userDetails,
                CustomQueries = valuesCustomQueries.OrderBy(x => x.QueryName).ToList()
            };

            return dropDownValues;
        }

        private ReportsSettingsView CreateDropDownValuesSaved(int memberId)
        {
            var reportsSettings = Uow.ReportsSettingsRepository.GetQueriesByMemberIdWithIncludes(memberId).FirstOrDefault(x => x.IsCurrentQuery);

            var dropDownsValuesSavedList = CreateReportsSettingsEntity(reportsSettings, true);

            return dropDownsValuesSavedList;
        }

        private ReportsSettingsView CreateReportsSettingsEntity(ReportsSettings defaultReportSettings, bool isDefaultQuery)
        {
            var dropDownsDefaultValuesSaved = new ReportsSettingsView();

            // Group By Date as default.
            dropDownsDefaultValuesSaved.GroupById = defaultReportSettings?.GroupById ?? (int) Constants.ReportsGroupBy.Date;

            // Show all columns as default.
            dropDownsDefaultValuesSaved.ShowColumnIds = defaultReportSettings?.FilterShowColumnIds == null
                ? new[]
                {
                    (int) ReportsExportService.ShowColumnModelIds.ShowEstimatedTime,
                    (int) ReportsExportService.ShowColumnModelIds.ShowDate,
                    (int) ReportsExportService.ShowColumnModelIds.ShowNotes,
                    (int) ReportsExportService.ShowColumnModelIds.ShowStartFinish
                }
                : ConvertStringToArrayOfInts(defaultReportSettings.FilterShowColumnIds);

            if (defaultReportSettings != null)
            {
                dropDownsDefaultValuesSaved.DateFrom = defaultReportSettings.DateFrom;
                dropDownsDefaultValuesSaved.DateTo = defaultReportSettings.DateTo;

                dropDownsDefaultValuesSaved.ClientIds = ConvertStringToArrayOfNullableInts(defaultReportSettings.FilterClientIds);
                dropDownsDefaultValuesSaved.ProjectIds = ConvertStringToArrayOfInts(defaultReportSettings.FilterProjectIds);
                dropDownsDefaultValuesSaved.MemberIds = ConvertStringToArrayOfInts(defaultReportSettings.FilterMemberIds);
                dropDownsDefaultValuesSaved.QueryName = defaultReportSettings.QueryName;
                dropDownsDefaultValuesSaved.QueryId = isDefaultQuery ? null : defaultReportSettings?.Id;
            }

            return dropDownsDefaultValuesSaved;
        }

        private static int[] ConvertStringToArrayOfInts(string sourceString)
        {
            return !string.IsNullOrEmpty(sourceString)
                ? sourceString.Split(',').Select(int.Parse).ToArray()
                : null;
        }

        private static int?[] ConvertStringToArrayOfNullableInts(string sourceString)
        {
            return !string.IsNullOrEmpty(sourceString)
                ? sourceString.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (int?) Convert.ToInt32(x)).ToArray()
                : null;
        }
    }
}