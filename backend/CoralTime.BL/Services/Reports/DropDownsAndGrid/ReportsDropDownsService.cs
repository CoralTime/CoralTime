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
        readonly List<ReportsDropDownGroupBy> DropDownGroupBy = new List<ReportsDropDownGroupBy>
        {
            //new ReportsDropDownGroupBy
            //{
            //    GroupById = (int) Constants.ReportsGroupBy.None,
            //    GroupByDescription = Constants.ReportsGroupBy.None.ToString()
            //},

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
            var user = Uow.UserRepository.GetRelatedUserByName(userName);
            var memberByUserName = Uow.MemberRepository.LinkedCacheGetByName(userName);

            var reportDropDowns = new ReportsDropDownsView
            {
                Values = CreateDropDownValues(memberByUserName),
                ValuesSaved = CreateDropDownValuesSaved(memberByUserName)
            };

            return reportDropDowns;
        }

        private ReportsDropDownValues CreateDropDownValues(Member memberByUserName)
        {
            var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();
            var memberRoleId = Uow.ProjectRoleRepository.GetMemberRoleId();

            var projectsOfClients = new List<Project>();
            var clientsFromProjectOfClients = new List<Client>();
            var members = Uow.MemberRepository.LinkedCacheGetList();

            #region GetProjects allProjectsForAdmin or projectsWithAssignUsersAndPublicProjects.

            if (memberByUserName.User.IsAdmin)
            {
                var allProjectsForAdmin = Uow.ProjectRepository.LinkedCacheGetList().ToList();
                projectsOfClients = allProjectsForAdmin;
            }
            else
            {
                var projectsWithAssignUsersAndPublicProjects = Uow.ProjectRepository.LinkedCacheGetList()
                    .Where(x => x.MemberProjectRoles.Select(z => z.MemberId).Contains(memberByUserName.Id) || !x.IsPrivate).ToList();

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
                        RoleId = project.MemberProjectRoles.FirstOrDefault(r => r.MemberId == memberByUserName.Id)?.RoleId ?? 0,
                        IsProjectActive = project.IsActive,
                    };

                    #region Set all users at Project constrain only for: Admin, Manager at this project.

                    var isManagerOnProject = project.MemberProjectRoles.Exists(r => r.MemberId == memberByUserName.Id && r.RoleId == managerRoleId);

                    if (memberByUserName.User.IsAdmin || isManagerOnProject)
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
                CurrentUserFullName = memberByUserName.FullName,
                CurrentUserId = memberByUserName.Id,
                IsAdminCurrentUser = memberByUserName.User.IsAdmin,
                IsManagerCurrentUser = memberByUserName.User.IsManager,
            };

            var dropDownValues = new ReportsDropDownValues
            {
                Filters = reportClientView,
                GroupBy = DropDownGroupBy,
                ShowColumns = ReportsExportService.showColumnsInfo,
                UserDetails = userDetails,
            };

            return dropDownValues;
        }

        private RequestReportsSettings CreateDropDownValuesSaved(Member memberByUserName)
        {
            var dropDownsValuesSaved = new RequestReportsSettings();

            var reportsSettings = Uow.ReportsSettingsRepository.GetQueryByMemberIdWithIncludes(memberByUserName.Id);

            if (reportsSettings != null)
            {
                dropDownsValuesSaved.DateFrom = reportsSettings.DateFrom;
                dropDownsValuesSaved.DateTo = reportsSettings.DateTo;
                dropDownsValuesSaved.GroupById = reportsSettings.GroupById;

                if (!string.IsNullOrEmpty(reportsSettings.ClientIds))
                {
                    dropDownsValuesSaved.ClientIds = reportsSettings.ClientIds?.Split(',')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => (int?)Convert.ToInt32(x))
                        .ToArray();
                }

                if (!string.IsNullOrEmpty(reportsSettings.ProjectIds))
                {
                    dropDownsValuesSaved.ProjectIds = reportsSettings.ProjectIds?.Split(',').Select(int.Parse).ToArray();
                }

                if (!string.IsNullOrEmpty(reportsSettings.MemberIds))
                {
                    dropDownsValuesSaved.MemberIds = reportsSettings.MemberIds?.Split(',').Select(int.Parse).ToArray();
                }

                if (!string.IsNullOrEmpty(reportsSettings.ShowColumnIds))
                {
                    dropDownsValuesSaved.ShowColumnIds = reportsSettings.ShowColumnIds?.Split(',').Select(int.Parse).ToArray();
                }
            }
            
            return dropDownsValuesSaved;
        }
    }
}