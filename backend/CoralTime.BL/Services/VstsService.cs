using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Models.Vsts;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.TimeEntries;
using CoralTime.ViewModels.Vsts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CoralTime.BL.Services
{
    public class VstsService : IVstsService, IVstsAdminService
    {
        private readonly UnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly ITimeEntryService _timeEntryService;

        public VstsService(UnitOfWork uow, IConfiguration config, ITimeEntryService timeEntryService)
        {
            _uow = uow;
            _config = config;
            _timeEntryService = timeEntryService;
        }

        public int? GetProjectIdByVstsProjectId(string projectId)
        {
            return _uow.VstsProjectRepository.GetQuery().Single(x => x.VstsProjectId == projectId)?.ProjectId;
        }

        public List<VstsTask> GetTasksByProject(string projectName)
        {
            var projectId = GetProjectIdByVstsProjectId(projectName);
            return _uow.TaskTypeRepository.LinkedCacheGetList()
                .Where(x => (x.ProjectId == null || x.ProjectId == projectId) && x.IsActive)
                .Select(x => new VstsTask
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
        }

        public int? GetTaskId(string taskName, string projectName)
        {
            return _uow.TaskTypeRepository.LinkedCacheGetList()
                .FirstOrDefault(x => (x.ProjectId == null || x.Project.Name == projectName) && x.IsActive && x.Name == taskName)?.Id;
        }

        public int? GetMemberIdByName(string userName)
        {
            return _uow.MemberRepository.LinkedCacheGetByUserName(userName)?.Id;
        }

        public Member GetVstsMember(string token)
        {
            var parsedToken = ValidateToken(token);
            if (token != null)
            {
                var nameId = parsedToken.Payload["nameid"].ToString();
                var user = _uow.VstsUserRepository.GetUserByVstsNameId(nameId);
                if (user != null)
                {
                    var member = _uow.MemberRepository.LinkedCacheGetByUserName(user.UserName);
                    return member;
                }
            }
            return null;
        }

        public bool SaveTimeEntry(TimeEntryView vstsTimeEnry, Member member)
        {
            if (member == null)
                return false;

            return _timeEntryService.Create(vstsTimeEnry, member) != null;
        }

        public List<VstsTimeEntry> GetTimeEntriesByWorkItemId(string vstsProjectId, string workItemId)
        {
            if (string.IsNullOrEmpty(workItemId) || string.IsNullOrEmpty(vstsProjectId))
            {
                return null;
            }
            var projectId = _uow.VstsProjectRepository.GetQuery(withIncludes: false)
                .FirstOrDefault(x => x.VstsProjectId == vstsProjectId)?.ProjectId;

            if (projectId == null)
            {
                return null;
            }

            var timeEntries = _uow.TimeEntryRepository.GetQuery()
                .Where(x => x.ProjectId == projectId && x.WorkItemId == workItemId)
                .Select(x => new VstsTimeEntry
                {
                    Date = x.Date,
                    Description = x.Description,
                    MemberId = x.MemberId,
                    MemberName = x.Member.FullName,
                    ProjectId = x.ProjectId,
                    TaskId = x.TaskTypesId,
                    TaskName = x.TaskType.Name,
                    WorkItemId = x.WorkItemId,
                    TimeOptions = new TimeOptions
                    {
                        IsFromToShow = x.IsFromToShow,
                        TimeTimerStart = x.TimeTimerStart
                    },
                    TimeValues = new TimeValuesView
                    {
                        TimeActual = x.TimeActual,
                        TimeEstimated = x.TimeEstimated,
                        TimeFrom = x.TimeFrom,
                        TimeTo = x.TimeTo
                    }
                })
                .OrderBy(y => y.Date)
                .ThenBy(z => z.MemberName)
                .ToList();

            return timeEntries;
        }

        public JwtSecurityToken ValidateToken(string issuedToken)
        {
            if (issuedToken == null)
            {
                return null;
            }

            //var secret = _config["VstsExtensionSecret"]; // your extension's secret

            //var validationParameters = new TokenValidationParameters()
            //{
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            //    ValidateIssuer = true,
            //    RequireSignedTokens = false,
            //    RequireExpirationTime = true,
            //    ValidateLifetime = true,
            //    ValidateAudience = false,
            //    ValidateActor = false,
            //    ValidateIssuerSigningKey = true,
            //    ValidIssuer = "app.vssps.visualstudio.com"
            //};

            var tokenHandler = new JwtSecurityTokenHandler();

            //var principal = tokenHandler.ValidateToken(issuedToken, validationParameters, out var valiadtedToken);
            //return valiadtedToken as JwtSecurityToken;
            return tokenHandler.ReadJwtToken(issuedToken);
        }

        public VstsSetup GetVstsSetupInfo(VstsSetup vstsSetup)
        {
            var project = _uow.VstsProjectRepository.GetQuery(withIncludes: false)
                .SingleOrDefault(x => x.VstsProjectId == vstsSetup.VstsProjectId);

            var user = _uow.VstsUserRepository.GetQuery()
                .SingleOrDefault(x => x.VstsUserId == vstsSetup.VstsUserId)?.User;

            var errors = new List<string>();

            if (project != null)
            {
                vstsSetup.ProjectId = project.ProjectId;
            }
            else
            {
                errors.Add("Project not found");
            }

            if (user != null)
            {
                vstsSetup.MemberId = _uow.MemberRepository.LinkedCacheGetByUserId(user.Id).Id;
            }
            else
            {
                errors.Add("User not found");
            }

            if (errors.Count != 0)
            {
                vstsSetup.Errors = errors;
            }
            return vstsSetup;
        }

        #region IVstsAdminService

        public void UpdateVstsProjects()
        {
            var existVstsProjects = _uow.VstsProjectRepository.GetQuery(withIncludes: false, asNoTracking: true).ToList();
            foreach (var item in existVstsProjects)
            {
                var projectUrl = item.VstsCompanyUrl + Constants.VstsProjectsUrl;
                var projectResponce = GetVstsData(url: projectUrl, personalaccesstoken: item.VstsPat);
                var projects = JsonConvert.DeserializeObject<VstsProjectList>(projectResponce);
                foreach (var vstsProject in projects.Value)
                {
                    // update existing records
                    var project = _uow.VstsProjectRepository.GetQuery(withIncludes: false)
                        .SingleOrDefault(x => x.VstsProjectId == vstsProject.Id || x.VstsProjectName == vstsProject.Name);
                    if (project != null)
                    {
                        project.VstsProjectId = vstsProject.Id;
                        project.VstsProjectName = vstsProject.Name;
                        continue;
                    }

                    // create new record for project with the same name
                    var existProject = _uow.ProjectRepository.GetQuery()
                        .SingleOrDefault(x => x.Name == vstsProject.Name);
                    if (existProject != null)
                    {
                        var newVstsProject = new DAL.Models.Vsts.VstsProject
                        {
                            ProjectId = existProject.Id,
                            VstsProjectId = vstsProject.Id,
                            VstsProjectName = vstsProject.Name,
                            VstsCompanyUrl = item.VstsCompanyUrl,
                            VstsPat = item.VstsPat
                        };
                        _uow.VstsProjectRepository.Insert(newVstsProject);
                    }
                }
                _uow.Save();
            }
        }

        public void UpdateVstsUsers()
        {
            var allMembers = new List<VstsMember>();
            var projects = _uow.VstsProjectRepository.GetQuery(withIncludes: false).ToList();
            foreach (var project in projects)
            {
                var teamUrl = $"{project.VstsCompanyUrl}{Constants.VstsProjectsUrl}/{project.VstsProjectId}{Constants.VstsTeamsUrl}";
                var teamResponce = GetVstsData(url: teamUrl, personalaccesstoken: project.VstsPat);
                var teams = JsonConvert.DeserializeObject<VstsTeamList>(teamResponce);
                foreach (var team in teams.Value)
                {
                    var memberUrl = team.Url + Constants.VstsMembersUrl;
                    var memberResponce = GetVstsData(url: memberUrl, personalaccesstoken: project.VstsPat);
                    var members = JsonConvert.DeserializeObject<VstsMemberList>(memberResponce);
                    members.Value.ForEach(x => allMembers.Add(x));
                }
            }

            foreach (var vstsMember in allMembers
                                        .Select(x => new { x.Identity.UniqueName, x.Identity.Id })
                                        .Distinct())
            {
                var user = _uow.VstsUserRepository.GetQuery()
                    .SingleOrDefault(x => x.VstsUserId == vstsMember.Id);
                var existUser = _uow.UserRepository.GetQuery(withIncludes: false)
                    .SingleOrDefault(x => x.UserName == vstsMember.UniqueName);

                if (user == null)
                {
                    if (existUser != null)
                    {
                        var newVstsUser = new VstsUser
                        {
                            UserId = existUser.Id,
                            VstsUserId = vstsMember.Id,
                            VstsUserName = vstsMember.UniqueName
                        };
                        _uow.VstsUserRepository.Insert(newVstsUser); // create new records
                    }
                }
                else
                {
                    if (user != null && user.User.UserName != vstsMember.UniqueName)
                    {
                        _uow.VstsUserRepository.Delete(user);  // remove wrong records
                    }
                }
            }
            _uow.Save();
        }

        private static string GetVstsData(string url, string personalaccesstoken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "", personalaccesstoken))));

                using (var response = client.GetAsync(url).Result)
                {
                    response.EnsureSuccessStatusCode();
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
        }

        #endregion IVstsAdminService

        #region VSTS Project Integration

        public IQueryable<VstsProjectIntegrationView> Get()
        {
            return _uow.VstsProjectRepository.GetQuery(withIncludes: true)
                    .Select(x => new VstsProjectIntegrationView
                    {
                        Id = x.Id,
                        ProjectId = x.ProjectId,
                        ProjectName = x.Project.Name,
                        VstsCompanyUrl = x.VstsCompanyUrl,
                        VstsPat = string.IsNullOrEmpty(x.VstsPat) ? null : x.VstsPat.Substring(0, 3) + "..." + x.VstsPat.Substring(x.VstsPat.Length - 3, 3),
                        VstsProjectId = x.VstsProjectId,
                        VstsProjectName = x.VstsProjectName
                    });
        }

        public bool Delete(int id)
        {
            var item = _uow.VstsProjectRepository.GetQuery(withIncludes: false)
                .SingleOrDefault(x => x.Id == id);
            _uow.VstsProjectRepository.Delete(item);
            return _uow.Save() > 0;
        }

        public VstsProjectIntegrationView Create(VstsProjectIntegrationView view)
        {
            var vstsProject = new DAL.Models.Vsts.VstsProject
            {
                ProjectId = view.ProjectId,
                VstsCompanyUrl = view.VstsCompanyUrl,
                VstsPat = view.VstsPat,
                VstsProjectId = view.VstsProjectId,
                VstsProjectName = view.VstsProjectName
            };

            _uow.VstsProjectRepository.Insert(vstsProject);
            var res = _uow.Save();
            if (res > 0)
            {
                var model = _uow.VstsProjectRepository.GetQuery(withIncludes: true)
                    .FirstOrDefault(x => x.Id == vstsProject.Id);
                return GetVstsProjectView(view.Id);
            }
            return null;
        }

         private VstsProjectIntegrationView GetVstsProjectView(int id)
        {
            var x = _uow.VstsProjectRepository.GetQuery(withIncludes: true)
                    .FirstOrDefault(z => z.Id == id);

            return new VstsProjectIntegrationView
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectName = x.Project.Name,
                VstsCompanyUrl = x.VstsCompanyUrl,
                VstsPat = string.IsNullOrEmpty(x.VstsPat) ? null : x.VstsPat.Substring(0, 3) + "..." + x.VstsPat.Substring(x.VstsPat.Length - 3, 3),
                VstsProjectId = x.VstsProjectId,
                VstsProjectName = x.VstsProjectName
            }; 
        }

        public VstsProjectIntegrationView Update(VstsProjectIntegrationView view)
        {
            var item = _uow.VstsProjectRepository.GetQuery(withIncludes: false)
                .SingleOrDefault(x => x.Id == view.Id);

            item.ProjectId = view.ProjectId;
            item.VstsCompanyUrl = view.VstsCompanyUrl;
            item.VstsProjectName = view.VstsProjectName;
            item.VstsPat = string.IsNullOrEmpty(view.VstsPat) ? item.VstsPat : view.VstsPat;

            var res = _uow.Save();
            if (res > 0)
            {
                var model = _uow.VstsProjectRepository.GetQuery(withIncludes: true)
                    .FirstOrDefault(x => x.Id == view.Id);
                return GetVstsProjectView(view.Id);
            }
            return null;
        }

        #endregion VSTS Project Integration
    }
}