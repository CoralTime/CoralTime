using CoralTime.BL.Interfaces;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.TimeEntries;
using CoralTime.ViewModels.Vsts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace CoralTime.BL.Services
{
    public class VstsService : IVstsService
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
            var secret = _config["VstsExtensionSecret"]; // your extension's secret

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
    }
}