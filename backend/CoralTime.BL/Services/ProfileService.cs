using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.DateFormat;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Projects.Profile;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.BL.Services
{
    public class ProfileService : BaseService, IProfileService
    {
        private readonly IConfiguration _config;
        private readonly IMemberService _memberService;
        private readonly bool _isDemo;
        private readonly IImageService _avatarService;

        public ProfileService(UnitOfWork uow, IConfiguration config, IHttpContextAccessor httpContextAccessor, IMapper mapper, IMemberService memberService, IImageService avatarService)
            : base(uow, mapper)
        {
            _config = config;
            _memberService = memberService;
            _isDemo = bool.Parse(_config["DemoSiteMode"]);
            _avatarService = avatarService;
        }

        public DateConvert[] GetDateFormats()
        {
            return DateFormats;
        }

        public List<ProfileProjectView> GetMemberProjects()
        {
            var projects = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(r => r.MemberId == BaseMemberImpersonated.Id && r.Project.IsActive)
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
                return projects.Select(project => new ProfileProjectView
                {
                    Id = project.Id,
                    ManagersNames = allRolesForAllProjects
                        .Where(x => x.ProjectId == project.Id && x.RoleId == Uow.ProjectRoleRepository.GetManagerRoleId())
                        .Select(x => x.Member.FullName)
                        .ToArray(),
                    MemberCount = allRolesForAllProjects.Count(x => x.ProjectId == project.Id),
                    Name = project.Name,
                    IsPrivate = project.IsPrivate,
                    Color = project.Color,
                    IsPrimary = project.Id == BaseMemberImpersonated.DefaultProjectId
                }).ToList();
            }

            return null;
        }

        public IEnumerable<ProjectMembersView> GetProjectMembers(int projectId)
        {
            var project = Uow.ProjectRepository.LinkedCacheGetById(projectId);
            if (project == null || !project.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"The project with id {projectId} not found or is not active");
            }

            var userHaveAccess = BaseMemberImpersonated.User.IsAdmin
                || !project.IsPrivate
                || Uow.MemberProjectRoleRepository.LinkedCacheGetList().Exists(r => r.ProjectId == projectId && r.MemberId == BaseMemberImpersonated.Id);

            if (!userHaveAccess)
            {
                throw new CoralTimeForbiddenException($"User with userName {BaseMemberImpersonated.User.UserName} has no access to project with id {projectId}");
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
            var memberByName = Uow.MemberRepository.GetQueryByMemberId(BaseMemberImpersonated.Id);
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

            var memberByName = Uow.MemberRepository.GetQueryByMemberId(BaseMemberImpersonated.Id);
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
    }
}