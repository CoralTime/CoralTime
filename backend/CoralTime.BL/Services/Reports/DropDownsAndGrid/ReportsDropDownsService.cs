using CoralTime.Common.Constants;
using CoralTime.DAL.ConvertersViews.ExstensionsMethods;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.ReportsDropwDowns;
using CoralTime.ViewModels.Reports.Request;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportService
    {
        public ReportDropDownView ReportsDropDowns(string userName)
        {
            var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();
            var memberRoleId = Uow.ProjectRoleRepository.GetMemberRoleId();
            var currentUserByName = Uow.MemberRepository.LinkedCacheGetByName(userName);

            var reportClientViewByUserId = new List<ReportClientView>();

            var projectsOfClients = new List<Project>();
            var clientsFromProjectOfClients = new List<Client>();
            var members = Uow.MemberRepository.LinkedCacheGetList();

            #region GetProjects allProjectsForAdmin or projectsWithAssignUsersAndPublicProjects.

            if (currentUserByName.User.IsAdmin)
            {
                var allProjectsForAdmin = Uow.ProjectRepository.LinkedCacheGetList().ToList();
                projectsOfClients = allProjectsForAdmin;
            }
            else
            {
                var projectsWithAssignUsersAndPublicProjects = Uow.ProjectRepository.LinkedCacheGetList()
                    .Where(x => x.MemberProjectRoles.Select(z => z.MemberId).Contains(currentUserByName.Id) || !x.IsPrivate).ToList();

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
                        RoleId = project.MemberProjectRoles.FirstOrDefault(r => r.MemberId == currentUserByName.Id)?.RoleId ?? 0,
                        IsProjectActive = project.IsActive,
                    };

                    #region Set all users at Project constrain only for: Admin, Manager at this project.

                    var isManagerOnProject = project.MemberProjectRoles.Exists(r => r.MemberId == currentUserByName.Id && r.RoleId == managerRoleId);

                    if (currentUserByName.User.IsAdmin || isManagerOnProject)
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

            reportClientViewByUserId = reportClientView;

            // Create responce with all projecs and all users of thise projects and info about current user.
            var reportsDetail = new ReportDropDownView
            {
                CurrentUserFullName = currentUserByName.FullName,
                CurrentUserId = currentUserByName.Id,
                IsAdminCurrentUser = currentUserByName.User.IsAdmin,
                IsManagerCurrentUser = currentUserByName.User.IsManager,
                ClientsDetails = reportClientViewByUserId
            };

            return reportsDetail;
        }

        public List<ReportsDropDownGroupBy> ReportsDropDownGroupBy()
        {
            var dropDownGroupBy = new List<ReportsDropDownGroupBy>
            {
                //new ReportsDropDownGroupBy
                //{
                //    GroupById = (int) Constants.ReportsGroupBy.None,
                //    GroupByDescription = Constants.ReportsGroupBy.None.ToString()
                //},

                new ReportsDropDownGroupBy
                {
                    GroupById = (int) Constants.ReportsGroupBy.Project,
                    GroupByDescription = Constants.ReportsGroupBy.Project.ToString()
                },

                new ReportsDropDownGroupBy
                {
                    GroupById = (int) Constants.ReportsGroupBy.User,
                    GroupByDescription = Constants.ReportsGroupBy.User.ToString()
                },

                new ReportsDropDownGroupBy
                {
                    GroupById = (int) Constants.ReportsGroupBy.Date,
                    GroupByDescription = Constants.ReportsGroupBy.Date.ToString()
                },

                new ReportsDropDownGroupBy
                {
                    GroupById = (int) Constants.ReportsGroupBy.Client,
                    GroupByDescription = Constants.ReportsGroupBy.Client.ToString()
                }
            };

            return dropDownGroupBy;
        }
    }
}