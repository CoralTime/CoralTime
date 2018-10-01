using System;
using CoralTime.Common.Constants;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using CoralTime.Common.Helpers;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertViewToModel;
using CoralTime.DAL.Models.ReportsSettings;
using CoralTime.ViewModels.Reports.Request.ReportsSettingsView;
using CoralTime.ViewModels.Reports.Responce.DropDowns;
using DatesStaticIds = CoralTime.Common.Constants.Constants.DatesStaticIds;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportsService
    {
        #region Values of groupByInfo, showColumnsInfo, datesStaticInfo.

        private readonly ReportCommonDropDownsView[] groupByInfo =
        {
            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ReportsGroupByIds.Project,
                Description = Constants.ReportsGroupByIds.Project.ToString()
            },

            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ReportsGroupByIds.User,
                Description = Constants.ReportsGroupByIds.User.ToString()
            },

            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ReportsGroupByIds.Date,
                Description = Constants.ReportsGroupByIds.Date.ToString()
            },

            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ReportsGroupByIds.Client,
                Description = Constants.ReportsGroupByIds.Client.ToString()
            }
        };

        private readonly ReportCommonDropDownsView[] showColumnsInfo =
        {
            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ShowColumnModelIds.ShowEstimatedTime,
                Description = "Show Estimated Hours"
            },
            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ShowColumnModelIds.ShowDate,
                Description = "Show Date"
            },
            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ShowColumnModelIds.ShowNotes,
                Description = "Show Notes"
            },
            new ReportCommonDropDownsView
            {
                Id = (int) Constants.ShowColumnModelIds.ShowStartFinish,
                Description = "Show Start/Finish Time"
            }
        };

        #endregion

        public ReportDropDownView GetReportsDropDowns()
        {
            return new ReportDropDownView
            {
                CurrentQuery = GetCurrentQuery() ?? CreateQueryWithDefaultValues(),
                Values = CreateDropDownValues()
            };
        }

        private ReportDropDownValues CreateDropDownValues()
        {
            return new ReportDropDownValues
            {
                Filters = CreateFilters(),
                UserDetails = CreateUserDetails(),
                CustomQueries = GetCustomQueries().OrderBy(x => x.QueryName).ToList(),
                GroupBy = groupByInfo,
                ShowColumns = showColumnsInfo,
                DateStatic = GetDatesStaticInfo()
            };
        }

        private List<Client> CreateClients()
        {
            var projects = new List<Project>();
            var clients = new List<Client>();

            #region GetProjects allProjectsForAdmin or projectsWithAssignUsersAndPublicProjects.

            if (BaseMemberImpersonated.User.IsAdmin)
            {
                var allProjectsForAdmin = Uow.ProjectRepository.LinkedCacheGetList().ToList();
                projects = allProjectsForAdmin;
            }
            else
            {
                var projectsWithAssignUsersAndPublicProjects = Uow.ProjectRepository.LinkedCacheGetList()
                    .Where(x => x.MemberProjectRoles.Select(z => z.MemberId).Contains(BaseMemberImpersonated.Id) || !x.IsPrivate)
                    .ToList();

                projects = projectsWithAssignUsersAndPublicProjects;
            }

            #endregion

            #region Get Clients from Projects of clients.

            // 1. Get all clients from targeted projects where project is assign to client.
            var clientsWithProjects = projects.Where(project => project.Client != null)
                .Select(project => project.Client)
                .Distinct()
                .Select(client => new Client
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                    IsActive = client.IsActive,
                    Description = client.Description,
                    Projects = new List<Project>(client.Projects.Where(projectOfClient => projects.Select(project => project.Id).Contains(projectOfClient.Id)).ToList())
                }).ToList();

            clients.AddRange(clientsWithProjects);

            // 2. Get all projects where project is not assign to client and create client "WithoutClients" that we add projects to it.
            var hasClientsWithoutProjects = projects.Where(x => x.Client == null).Any();
            if (hasClientsWithoutProjects)
            {
                var clientWithoutProjects = new Client
                {
                    Id = Constants.WithoutClient.Id,
                    Name = Constants.WithoutClient.Name,
                    IsActive = true,
                    Projects = new List<Project>(projects.Where(x => x.Client == null).ToList())
                };

                clients.Add(clientWithoutProjects);
            }

            #endregion

            return clients;
        }

        private List<ReportClientView> CreateFilters()
        {
            var reportClientView = new List<ReportClientView>();
            var members = Uow.MemberRepository.LinkedCacheGetList();

            var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();
            var memberRoleId = Uow.ProjectRoleRepository.GetMemberRoleId();

            var clients = CreateClients();
            foreach (var client in clients)
            {
                var reportProjectViewByUserId = new List<ReportProjectView>();

                foreach (var project in client.Projects)
                {
                    var reportProjectView = new ReportProjectView
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        RoleId = project.MemberProjectRoles.FirstOrDefault(r => r.MemberId == ReportMemberImpersonated.Id)?.RoleId ?? 0,
                        IsProjectActive = project.IsActive,
                    };

                    #region Set all users at Project constrain only for: Admin, Manager at this project.

                    var isManagerOnProject = project.MemberProjectRoles.Exists(r => r.MemberId == ReportMemberImpersonated.Id && r.RoleId == managerRoleId);

                    if (ReportMemberImpersonated.User.IsAdmin || isManagerOnProject)
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

            return reportClientView;
        }

        private ReportUserDetails CreateUserDetails()
        {
            return new ReportUserDetails
            {
                CurrentUserFullName = ReportMemberImpersonated.FullName,
                CurrentUserId = ReportMemberImpersonated.Id,
                IsAdminCurrentUser = ReportMemberImpersonated.User.IsAdmin,
                IsManagerCurrentUser = ReportMemberImpersonated.User.IsManager,
            };
        }

        private List<ReportsSettingsView> GetCustomQueries()
        {
            var customQueries = Uow.ReportsSettingsRepository.LinkedCacheGetByMemberId(ReportMemberImpersonated.Id).Where(x => x.QueryName != null);

            return customQueries.Select(x => x.GetView(GetDayOfWeek(ReportMemberImpersonated.WeekStart))).ToList();
        }

        private ReportDropDownsDateStaticExtendView CreateDateStaticExtend(int? dateStaticId)
        {
            var datesStaticInfo = GetDatesStaticInfo();
            var dateStaticPeriod = datesStaticInfo.FirstOrDefault(x => x.Id == dateStaticId);
            
            if (dateStaticPeriod == null) 
                throw new CoralTimeDangerException($"Incorrect DateStaticId = {dateStaticId}");
            
            var dateStaticExtend = new ReportDropDownsDateStaticExtendView
            {
                DateStatic = datesStaticInfo,

                DateFrom = dateStaticPeriod.DateFrom,
                DateTo = dateStaticPeriod.DateTo
            };

            return dateStaticExtend;
            

        }
        
        private static DayOfWeek GetDayOfWeek(Constants.WeekStart day) => 
            (day == Constants.WeekStart.Monday)? DayOfWeek.Monday : DayOfWeek.Sunday;
        
        private ReportDropDownsDateStaticView[] GetDatesStaticInfo()
        {
            var companyReportStartOfWeek = (Constants.WeekStart) int.Parse(_config["CompanyReportStartOfWeek"]); 
            var memberDayOfWeekStart = GetDayOfWeek(companyReportStartOfWeek); 
            var today = CommonHelpers.GetPeriod(DatesStaticIds.Today);
            var yesterday = CommonHelpers.GetPeriod(DatesStaticIds.Yesterday);
            var thisWeek = CommonHelpers.GetPeriod(DatesStaticIds.ThisWeek, memberDayOfWeekStart);
            var thisMonth = CommonHelpers.GetPeriod(DatesStaticIds.ThisMonth);
            var thisYear = CommonHelpers.GetPeriod(DatesStaticIds.ThisYear);
            var lastWeek = CommonHelpers.GetPeriod(DatesStaticIds.LastWeek, memberDayOfWeekStart);
            var lastMonth = CommonHelpers.GetPeriod(DatesStaticIds.LastMonth);
            var lastYear = CommonHelpers.GetPeriod(DatesStaticIds.LastYear);
            
            ReportDropDownsDateStaticView[] datesStaticInfo =
            {
                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.Today,
                    Description = "Today",
                    DateFrom = today.DateFrom,
                    DateTo = today.DateTo
                },

                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.ThisWeek,
                    Description = "This Week",
                    DateFrom = thisWeek.DateFrom,
                    DateTo = thisWeek.DateTo
                },

                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.ThisMonth,
                    Description = "This Month",
                    DateFrom = thisMonth.DateFrom,
                    DateTo = thisMonth.DateTo
                },

                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.ThisYear,
                    Description = "This Year",
                    DateFrom = thisYear.DateFrom,
                    DateTo = thisYear.DateTo
                },

                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.Yesterday,
                    Description = "Yesterday",
                    DateFrom = yesterday.DateFrom,
                    DateTo = yesterday.DateTo
                },

                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.LastWeek,
                    Description = "Last Week",
                    DateFrom = lastWeek.DateFrom,
                    DateTo = lastWeek.DateTo
                },

                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.LastMonth,
                    Description = "Last Month",
                    DateFrom = lastMonth.DateFrom,
                    DateTo = lastMonth.DateTo
                },

                new ReportDropDownsDateStaticView
                {
                    Id = (int) DatesStaticIds.LastYear,
                    Description = "Last Year",
                    DateFrom = lastYear.DateFrom,
                    DateTo = lastYear.DateTo
                }
            };

            return datesStaticInfo;
        }

        private ReportsSettingsView GetCurrentQuery()
        {
            var reportsSettings = Uow.ReportsSettingsRepository.LinkedCacheGetByMemberId(ReportMemberImpersonated.Id).FirstOrDefault(x => x.IsCurrentQuery);

            return reportsSettings?.GetView(GetDayOfWeek(ReportMemberImpersonated.WeekStart));
        }

        private ReportsSettingsView CreateQueryWithDefaultValues()
        {
            var reportsSettingsView = new ReportsSettingsView().GetViewWithDefaultValues();

            var dateStaticExtend = CreateDateStaticExtend(reportsSettingsView.DateStaticId);
            reportsSettingsView.DateFrom = dateStaticExtend.DateFrom;
            reportsSettingsView.DateTo = dateStaticExtend.DateTo;

            var reportsSettings = new ReportsSettings().GetModel(reportsSettingsView, ReportMemberImpersonated.Id);

            Uow.ReportsSettingsRepository.Insert(reportsSettings);
            Uow.Save();
            Uow.ReportsSettingsRepository.LinkedCacheClear();

            reportsSettingsView = reportsSettings.GetView(GetDayOfWeek(ReportMemberImpersonated.WeekStart));

            return reportsSettingsView;
        }
    }
}