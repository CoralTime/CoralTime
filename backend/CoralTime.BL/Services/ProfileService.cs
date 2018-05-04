using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.ConvertModelToView.Profile;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.DateFormat;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.BL.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        private readonly IConfiguration _config;
        private readonly IMemberService _memberService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool _isDemo;
        private readonly IImageService _avatarService;

        public ProfileService(UnitOfWork uow, IConfiguration config, IHttpContextAccessor httpContextAccessor, IMapper mapper, IMemberService memberService, IImageService avatarService)
            : base(uow, mapper)
        {
            _config = config;
            _memberService = memberService;
            _httpContextAccessor = httpContextAccessor;
            _isDemo = bool.Parse(_config["DemoSiteMode"]);
            _avatarService = avatarService;
        }

        public DateConvert[] GetDateFormats()
        {
            return DateFormats;
        }

        public List<ProfileProjectView> GetMemberProjects()
        {
            var user = Uow.UserRepository.LinkedCacheGetByName(ImpersonatedUserName);
            if (user == null || !user.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"User with userName {ImpersonatedUserName} not found or is not active");
            }

            var member = Uow.MemberRepository.LinkedCacheGetByName(ImpersonatedUserName);
            if (member == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with user with userName {ImpersonatedUserName} not found");
            }

            var projects = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(r => r.MemberId == member.Id && r.Project.IsActive)
                .Select(r => r.Project)
                .ToList();

            var publicProjects = Uow.ProjectRepository.LinkedCacheGetList()
                .Where(p => !p.IsPrivate && p.IsActive);

            projects.AddRange(publicProjects);

            var allRolesForAllProjects = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                    .Where(r => projects.Select(p => p.Id).Contains(r.ProjectId) && r.Member.User.IsActive)
                    .ToArray();

            if (projects.Count > 0)
            {
                var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

                return projects.Select(project => new ProfileProjectView
                {
                    Id = project.Id,
                    ManagersNames = allRolesForAllProjects
                        .Where(x => x.ProjectId == project.Id && x.RoleId == managerRoleId)
                        .Select(x => x.Member.FullName)
                        .ToArray(),
                    MemberCount = allRolesForAllProjects.Count(x => x.ProjectId == project.Id),
                    Name = project.Name,
                    IsPrivate = project.IsPrivate,
                    Color = project.Color,
                    IsPrimary = project.Id == member.DefaultProjectId
                }).ToList();
            }

            return null;
        }

        public IEnumerable<ProjectMembersView> GetProjectMembers(int projectId)
        {
            var user = Uow.UserRepository.LinkedCacheGetByName(ImpersonatedUserName);
            if (user == null || !user.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"The user with userName {ImpersonatedUserName} not found or is not active");
            }

            var member = Uow.MemberRepository.LinkedCacheGetByName(ImpersonatedUserName);
            if (member == null)
            {
                throw new CoralTimeEntityNotFoundException($"The member with userName {ImpersonatedUserName} not found");
            }

            var project = Uow.ProjectRepository.LinkedCacheGetById(projectId);
            if (project == null || !project.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"The project with id {projectId} not found or is not active");
            }

            var userHaveAccess = user.IsAdmin
                || !project.IsPrivate
                || Uow.MemberProjectRoleRepository.LinkedCacheGetList().Exists(r => r.ProjectId == projectId && r.MemberId == member.Id);

            if (!userHaveAccess)
            {
                throw new CoralTimeForbiddenException($"User with userName {ImpersonatedUserName} has no access to project with id {projectId}");
            }

            var projectMembers = new List<Member>();

            if (project.IsPrivate)
            {
                projectMembers = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                        .Where(r => r.ProjectId == projectId && r.Member.User.IsActive)
                        .Select(r => r.Member).ToList();
            }
            else
            {
                projectMembers = Uow.MemberRepository.LinkedCacheGetList().Where(m => m.User.IsActive).ToList();
            }

            var profileProjectMemberView = projectMembers.Select(x => x.GetViewProjectMembers(Mapper, _avatarService.GetUrlIcon(x.Id)));

            return profileProjectMemberView;
        }

        //public MemberView PatchNotifications(MemberNotificationView memberNotificationView)
        //{
        //    CheckRelatedEntities(ImpersonatedUserName, out var memberByName);
        //    memberByName = Uow.MemberRepository.GetQueryByUserName(ImpersonatedUserName);
        //    memberByName.SendEmailTime = memberNotificationView.SendEmailTime;
        //    memberByName.SendEmailDays = ConverterBitMask.DayOfWeekStringToInt(memberNotificationView.SendEmailDays);
        //    memberByName.IsWeeklyTimeEntryUpdatesSend = memberNotificationView.IsWeeklyTimeEntryUpdatesSend;

        //    Uow.MemberRepository.Update(memberByName);
        //    Uow.Save();

        //    Uow.MemberRepository.LinkedCacheClear();

        //    var urlIcon = _avatarService.GetUrlIcon(memberByName.Id);
        //    var memberView = memberByName.GetView(Mapper, urlIcon);

        //    return memberView;
        //}

        public MemberView PatchPreferences(MemberPreferencesView memberPreferencesView)
        {
            CheckRelatedEntities(ImpersonatedUserName, out var memberByName);
            memberByName = Uow.MemberRepository.GetQueryByUserName(ImpersonatedUserName);
            memberByName.DefaultProjectId = memberPreferencesView.DefaultProjectId;
            memberByName.DefaultTaskId = memberPreferencesView.DefaultTaskId;
            memberByName.DateFormatId = memberPreferencesView.DateFormatId;
            memberByName.TimeFormat = memberPreferencesView.TimeFormat;
            memberByName.WeekStart = (WeekStart)memberPreferencesView.WeekStart;
            memberByName.IsWeeklyTimeEntryUpdatesSend = memberPreferencesView.IsWeeklyTimeEntryUpdatesSend;

            Uow.MemberRepository.Update(memberByName);
            Uow.Save();

            Uow.MemberRepository.LinkedCacheClear();

            var urlIcon = _avatarService.GetUrlIcon(memberByName.Id);
            var memberView = memberByName.GetView(Mapper, urlIcon);

            return memberView;
        }

        public MemberView PatchPersonalInfo(MemberPersonalInfoView memberPreferencesView)
        {
            if (!EmailChecker.IsValidEmail(memberPreferencesView.Email))
            {
                throw new CoralTimeDangerException("Invalid email");
            }

            if (_isDemo)
            {
                throw new CoralTimeForbiddenException("Full name can't be changed on demo site");
            }

            CheckRelatedEntities(ImpersonatedUserName, out var memberByName);
            memberByName = Uow.MemberRepository.GetQueryByUserName(ImpersonatedUserName);
            memberByName.FullName = memberPreferencesView.FullName;
          
            Uow.MemberRepository.Update(memberByName);
            Uow.Save();

            _memberService.ChangeEmailByUserAsync(memberByName, memberPreferencesView.Email).GetAwaiter().GetResult();
            _memberService.UpdateUserClaims(memberByName.Id);

            Uow.MemberRepository.LinkedCacheClear();

            var urlIcon = _avatarService.GetUrlIcon(memberByName.Id);
            var memberView = memberByName.GetView(Mapper, urlIcon);

            return memberView;
        }

        private void CheckRelatedEntities(string userName, out Member relatedMemberByName)
        {
            GetRelatedUserByName(userName);
            relatedMemberByName = GetRelatedMemberByUserName(userName);
        }

        private ApplicationUser GetRelatedUserByName(string userName)
        {
            var relatedUserByName = Uow.UserRepository.LinkedCacheGetByName(userName);
            if (relatedUserByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"User {userName} not found.");
            }

            if (!relatedUserByName.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"User {userName} is not active.");
            }

            return relatedUserByName;
        }

        private Member GetRelatedMemberByUserName(string userName)
        {
            var relatedMemberByName = Uow.MemberRepository.LinkedCacheGetByName(userName);
            if (relatedMemberByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with userName {userName} not found.");
            }

            return relatedMemberByName;
        }
    }
}