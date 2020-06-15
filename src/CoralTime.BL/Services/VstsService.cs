using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Models.Vsts;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.TimeEntries;
using CoralTime.ViewModels.Vsts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CoralTime.Common.Helpers;

namespace CoralTime.BL.Services
{
    public class VstsService : IVstsService, IVstsAdminService
    {
        private readonly UnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly ITimeEntryService _timeEntryService;
        private readonly ILogger _logger;
        private readonly IImageService _imageService;

        public VstsService(UnitOfWork uow, IConfiguration config, ITimeEntryService timeEntryService, ILogger<VstsService> logger, IImageService imageService)
        {
            _uow = uow;
            _config = config;
            _timeEntryService = timeEntryService;
            _logger = logger;
            _imageService = imageService;
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
                return _uow.VstsUserRepository.GetUserByVstsNameId(nameId);
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
                    Description = StringHandler.RemoveMarkdown(x.Description),
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

            var memberId = _uow.VstsUserRepository.GetQuery()
                .SingleOrDefault(x => x.VstsUserId == vstsSetup.VstsUserId)?.MemberId;

            var errors = new List<string>();

            if (project != null)
            {
                vstsSetup.ProjectId = project.ProjectId;
            }
            else
            {
                errors.Add("Project not found");
            }

            if (memberId != null)
            {
                vstsSetup.MemberId = memberId;
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
                var projectResponce = GetVstsData(projectUrl, item.VstsPat, out bool isSuccessStatusCode);
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

        public bool UpdateVstsProject(int projectId)
        {
            var existVstsProject = _uow.VstsProjectRepository
                .GetQuery(withIncludes: false)
                .FirstOrDefault(x => x.Id == projectId);

            var projectUrl = existVstsProject.VstsCompanyUrl + Constants.VstsProjectsUrl;
            var projectResponce = GetVstsData(projectUrl, existVstsProject.VstsPat, out bool isSuccessStatusCode);

            if (!isSuccessStatusCode)
                return isSuccessStatusCode;

            var projects = JsonConvert.DeserializeObject<VstsProjectList>(projectResponce);
            foreach (var vstsProject in projects.Value)
            {
                // update existing records
                if (existVstsProject.VstsProjectId == vstsProject.Id || existVstsProject.VstsProjectName == vstsProject.Name)
                {
                    existVstsProject.VstsProjectId = vstsProject.Id;
                    existVstsProject.VstsProjectName = vstsProject.Name;
                    continue;
                }
            }
            _uow.Save();
            return isSuccessStatusCode;
        }

        public bool UpdateVstsUsersByProject(int projectId)
        {
            var project = _uow.VstsProjectRepository
                .GetQuery(withIncludes: false)
                .FirstOrDefault(x => x.Id == projectId);
            var allMembers = new List<VstsMember>();

            var teamUrl = $"{project.VstsCompanyUrl}{Constants.VstsProjectsUrl}/{project.VstsProjectId}{Constants.VstsTeamsUrl}";
            var teamResponce = GetVstsData(teamUrl, project.VstsPat, out bool isSuccessStatusCode);
            if (!isSuccessStatusCode)
                return isSuccessStatusCode;

            var teams = JsonConvert.DeserializeObject<VstsTeamList>(teamResponce);
            foreach (var team in teams.Value)
            {
                var memberUrl = team.Url + Constants.VstsMembersUrl;
                var memberResponce = GetVstsData(memberUrl, project.VstsPat, out isSuccessStatusCode);

                if (!isSuccessStatusCode)
                    return isSuccessStatusCode;

                var members = JsonConvert.DeserializeObject<VstsMemberList>(memberResponce);
                members.Value.ForEach(x => allMembers.Add(x));
            }

            foreach (var vstsMember in allMembers
                                        .Select(x => new { x.Identity.UniqueName, x.Identity.Id })
                                        .Distinct())
            {
                var user = _uow.VstsUserRepository.GetQuery(withIncludes: true)
                    .SingleOrDefault(x => x.VstsUserId == vstsMember.Id);
                var existMember = _uow.MemberRepository.GetQuery(withIncludes: false)
                    .SingleOrDefault(x => x.User.UserName == vstsMember.UniqueName);

                if (user == null)
                {
                    if (existMember != null)
                    {
                        var newVstsUser = new VstsUser
                        {
                            MemberId = existMember.Id,
                            VstsUserId = vstsMember.Id,
                            VstsUserName = vstsMember.UniqueName
                        };
                        _uow.VstsUserRepository.Insert(newVstsUser); // create new records
                        _uow.Save();

                        var vstsProjectUser = new VstsProjectUser
                        {
                            VstsProjectId = project.Id,
                            VstsUserId = newVstsUser.Id
                        };
                        _uow.VstsProjectUserRepository.Insert(vstsProjectUser);
                        _uow.Save();
                    }
                }
                else
                {
                    if (user != null && user.Member.User.UserName != vstsMember.UniqueName)
                    {
                        var itemsToDelete = _uow.VstsProjectUserRepository
                            .GetQuery()
                            .Where(x => x.VstsProjectId == project.Id && x.VstsUserId == user.Id)
                            .ToList();
                        _uow.VstsProjectUserRepository.DeleteRange(itemsToDelete);
                        _uow.VstsUserRepository.Delete(user);  
                        _uow.Save();
                    }
                    else
                    {
                        var item = _uow.VstsProjectUserRepository.GetQuery()
                            .FirstOrDefault(x => x.VstsProjectId == project.Id && x.VstsUserId == user.Id);
                        if (item == null)
                        {
                            var vstsProjectUser = new VstsProjectUser
                            {
                                VstsProjectId = project.Id,
                                VstsUserId = user.Id
                            };
                            _uow.VstsProjectUserRepository.Insert(vstsProjectUser);
                            _uow.Save();
                        }
                    }
                }
            }
            return isSuccessStatusCode;
        }

        public void UpdateVstsUsers()
        {
            var allMembers = new List<VstsMember>();
            var projects = _uow.VstsProjectRepository.GetQuery(withIncludes: false).ToList();
            foreach (var project in projects)
            {
                var teamUrl = $"{project.VstsCompanyUrl}{Constants.VstsProjectsUrl}/{project.VstsProjectId}{Constants.VstsTeamsUrl}";
                var teamResponce = GetVstsData(teamUrl, project.VstsPat, out bool isSuccessStatusCode);
                var teams = JsonConvert.DeserializeObject<VstsTeamList>(teamResponce);
                foreach (var team in teams.Value)
                {
                    var memberUrl = team.Url + Constants.VstsMembersUrl;
                    var memberResponce = GetVstsData(memberUrl, project.VstsPat, out isSuccessStatusCode);
                    var members = JsonConvert.DeserializeObject<VstsMemberList>(memberResponce);
                    members.Value.ForEach(x => allMembers.Add(x));
                }
            }

            foreach (var vstsMember in allMembers
                                        .Select(x => new { x.Identity.UniqueName, x.Identity.Id })
                                        .Distinct())
            {
                var user = _uow.VstsUserRepository.GetQuery(withIncludes: true)
                    .SingleOrDefault(x => x.VstsUserId == vstsMember.Id);
                var existMember = _uow.MemberRepository.GetQuery(withIncludes: false)
                    .SingleOrDefault(x => x.User.UserName == vstsMember.UniqueName);

                if (user == null)
                {
                    if (existMember != null)
                    {
                        var newVstsUser = new VstsUser
                        {
                            MemberId = existMember.Id,
                            VstsUserId = vstsMember.Id,
                            VstsUserName = vstsMember.UniqueName
                        };
                        _uow.VstsUserRepository.Insert(newVstsUser); // create new records
                    }
                }
                else
                {
                    if (user != null && user.Member.User.UserName != vstsMember.UniqueName)
                    {
                        _uow.VstsUserRepository.Delete(user);  // remove wrong records
                    }
                }
            }
            _uow.Save();
        }

        private string GetVstsData(string url, string personalaccesstoken, out bool isSuccessStatusCode)
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
                    isSuccessStatusCode = response.IsSuccessStatusCode;
                    var res = response.Content.ReadAsStringAsync().Result;
                    if (!isSuccessStatusCode)
                    {
                        _logger.LogWarning($"Server {url} returned response with status {response.StatusCode}. Content: {res}");
                    }                    
                    return res;
                }
            }
        }

        #endregion IVstsAdminService

        #region VSTS Project Integration

        public IQueryable<VstsProjectIntegrationView> Get()
        {
            return _uow.VstsProjectRepository.GetQuery()
                .Select(x=> new VstsProjectIntegrationView
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    ProjectName = x.Project.Name,
                    VstsCompanyUrl = x.VstsCompanyUrl,
                    VstsPat = string.IsNullOrEmpty(x.VstsPat) ? null : x.VstsPat.Substring(0, 3) + "..." + x.VstsPat.Substring(x.VstsPat.Length - 3, 3),
                    VstsProjectId = x.VstsProjectId,
                    VstsProjectName = x.VstsProjectName,
                    MembersCount = x.VstsProjectUsers.Count
                });             
        }

        public bool Delete(int id)
        {
            var projectToDelete = _uow.VstsProjectRepository.GetQuery(withIncludes: false)
                .SingleOrDefault(x => x.Id == id);

            var linksToDelete = _uow.VstsProjectUserRepository.GetQuery()
                .Where(x => x.VstsProjectId == projectToDelete.Id).ToList();
            var vstsUsersToDelete = _uow.VstsUserRepository.GetQuery()
                .Where(x => x.VstsProjectUsers.Count == 0)
                .ToList();
            _uow.VstsProjectUserRepository.DeleteRange(linksToDelete);
            _uow.VstsProjectRepository.Delete(projectToDelete);
            _uow.VstsUserRepository.DeleteRange(vstsUsersToDelete);
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
                return VstsProjectMapToView(
                    _uow.VstsProjectRepository.GetQuery()
                    .FirstOrDefault(x => x.Id == vstsProject.Id));
            }
            throw new CoralTimeSafeEntityException("ProjectIntegration was not created");
        }

        private VstsProjectIntegrationView VstsProjectMapToView(DAL.Models.Vsts.VstsProject x)
        {
            return new VstsProjectIntegrationView
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectName = x.Project.Name,
                VstsCompanyUrl = x.VstsCompanyUrl,
                VstsPat = string.IsNullOrEmpty(x.VstsPat) ? null : x.VstsPat.Substring(0, 3) + "..." + x.VstsPat.Substring(x.VstsPat.Length - 3, 3),
                VstsProjectId = x.VstsProjectId,
                VstsProjectName = x.VstsProjectName,
                MembersCount = x.VstsProjectUsers.Count
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
            //if (res > 0)
            //{
                return VstsProjectMapToView(
                    _uow.VstsProjectRepository.GetQuery()
                    .FirstOrDefault(x => x.Id == view.Id));
            //}
            //return null;
        }

        public IEnumerable<VstsMemberView> GetMembersByProjectId(int vstsProjectId)
        {
            var members =_uow.VstsProjectUserRepository
                .GetQuery()
                .Where(x => x.VstsProjectId == vstsProjectId)
                .Select(x => new VstsMemberView
                {
                    Id = x.Id,
                    MemberId = x.VstsUser.MemberId,
                    FullName = x.VstsUser.Member.FullName
                }).ToList();
            foreach (var item in members)
            {
                item.UrlIcon = _imageService.GetUrlIcon(item.MemberId);
            }
            return members;
        }

        #endregion VSTS Project Integration
    }
}