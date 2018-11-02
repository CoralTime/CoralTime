using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Models.Vsts;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.TimeEntries;
using CoralTime.ViewModels.Vsts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

        private TimeEntryView ConvertTimeEntryFromVsts(VstsTimeEntry vstsTimeEnry)
        {
            return new TimeEntryView
            {
                Description = vstsTimeEnry.Description,
                Date = vstsTimeEnry.Date,
                ProjectId = GetProjectIdByProjectName(vstsTimeEnry.ProjectName) ?? 0,
                TaskTypesId = GetTaskId(taskName: vstsTimeEnry.TaskName, projectName: vstsTimeEnry.ProjectName) ?? 0,
                MemberId = GetMemberIdByName(vstsTimeEnry.UserName) ?? 0,
                TimeOptions = new TimeOptions
                {
                    IsFromToShow = false,
                    TimeTimerStart = 0
                },
                TimeValues = new TimeValuesView
                {
                    TimeActual = vstsTimeEnry.TimeActual,
                    TimeEstimated = vstsTimeEnry.TimeEstimated
                }
            };
        }

        public int? GetProjectIdByProjectName(string projectName)
        {
            return _uow.ProjectRepository.LinkedCacheGetByName(projectName)?.Id;
        }

        public VstsTasks GetTasksByProject(string projectName)
        {
            var projectId = GetProjectIdByProjectName(projectName);
            var tasks = _uow.TaskTypeRepository.LinkedCacheGetList()
                .Where(x => (x.ProjectId == null || x.ProjectId == projectId) && x.IsActive)
                .Select(x => x.Name)
                .ToList();
            return new VstsTasks
            {
                Tasks = tasks
            };
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

        public Member GetVstsMember(string token, string id, string userName)
        {
            //var parsedToken = ValidateToken(token);
            if (token != null)
            {
                var member = _uow.MemberRepository.LinkedCacheGetByUserName(userName);
                return member;
            }
            return null;
        }

        public bool SaveTimeEntry(VstsTimeEntry vstsTimeEnry)
        {
            var timeEntry = ConvertTimeEntryFromVsts(vstsTimeEnry);
            return _timeEntryService.Create(timeEntry) != null;
        }

        public JwtSecurityToken ValidateToken(string issuedToken)
        {
            if (issuedToken == null)
            {
                return null;
            }
            var secret = _config["Vsts:VstsExtensionSecret"]; // your extension's secret

            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = false,
                RequireSignedTokens = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateActor = false,
                ValidateIssuerSigningKey = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(issuedToken, validationParameters, out var valiadtedToken);
            return valiadtedToken as JwtSecurityToken;
        }

        public void UpdateVstsProjects()
        {
            var projectUrl = GetVstsCompanyUrl() + Constants.VstsProjectsUrl;
            var projectResponce = GetVstsData(url: projectUrl, personalaccesstoken: GetPat());
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
                        VstsProjectName = vstsProject.Name
                    };
                    _uow.VstsProjectRepository.Insert(newVstsProject);
                }
            }
            _uow.Save();
        }

        public void UpdateVstsUsers()
        {
            var allMembers = new List<VstsMember>();
            var projectIds = _uow.VstsProjectRepository.GetQuery(withIncludes: false)
                .Select(x => x.VstsProjectId);
            foreach (var id in projectIds)
            {
                var teamUrl = $"{GetVstsCompanyUrl()}{Constants.VstsProjectsUrl}/{id}{Constants.VstsTeamsUrl}";
                var teamResponce = GetVstsData(url: teamUrl, personalaccesstoken: GetPat());
                var teams = JsonConvert.DeserializeObject<VstsTeamList>(teamResponce);
                foreach (var team in teams.Value)
                {
                    var memberUrl = team.Url + Constants.VstsMembersUrl;
                    var memberResponce = GetVstsData(url: memberUrl, personalaccesstoken: GetPat());
                    var members = JsonConvert.DeserializeObject<VstsMemberList>(memberResponce);
                    members.Value.ForEach(x => allMembers.Add(x));
                }
            }

            foreach (var vstsMember in allMembers)
            {
                var user = _uow.VstsUserRepository.GetQuery()
                    .SingleOrDefault(x => x.VstsUserId == vstsMember.Identity.Id);
                
                if (user != null && user.User.UserName != vstsMember.Identity.UniqueName)
                {
                    _uow.VstsUserRepository.Delete(user);  // remove wrong records
                }
                else
                {
                    var existUser = _uow.UserRepository.GetQuery(withIncludes: false)
                        .SingleOrDefault(x => x.UserName == vstsMember.Identity.UniqueName);
                    if (existUser != null)
                    {
                        var newVstsUser = new VstsUser
                        {
                            UserId = existUser.Id,
                            VstsUserId = vstsMember.Identity.Id
                        };
                        _uow.VstsUserRepository.Insert(newVstsUser); // create new records
                    }
                }
            }
            _uow.Save();
        }

        private string GetPat()=> _config["Vsts:VstsPatToken"];

        private string GetVstsCompanyUrl() => _config["Vsts:VstsCompanyUrl"];

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
    }
}