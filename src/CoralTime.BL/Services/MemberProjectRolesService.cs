using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.MemberProjectRoles;
using CoralTime.ViewModels.ProjectRole;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using System.Linq;
using CoralTime.DAL.Models.Member;
using CoralTime.Common.Constants;

namespace CoralTime.BL.Services
{
    public class MemberProjectRoleService : BaseService, IMemberProjectRoleService
    {
        private readonly IProjectService _projectService;
        private readonly IImageService _avatarService;

        public MemberProjectRoleService(UnitOfWork uow, IProjectService projectService, IMapper mapper, IImageService avatarService)
            : base(uow, mapper)
        {
            _projectService = projectService;
            _avatarService = avatarService;
        }

        public IEnumerable<MemberProjectRoleView> GetAllProjectRoles()
        {
            var memberProjectRoleView = new List<MemberProjectRoleView>();

            //var memberByName = Uow.MemberRepository.LinkedCacheGetByName(ImpersonatedUserName);

            // Get MemberProjectRoles from DB.
            var allMemberProjectRoles = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Select(x => x.GetView(Mapper, _avatarService.GetUrlIcon(x.MemberId)))
                .ToList();

            #region Constrain for admin:

            var managesAll = Authorize(BaseMemberImpersonated, Constants.PolicyManagesAllProjects);

            if (managesAll)
            {
                // Add MemberProjectRoles from db to result.
                memberProjectRoleView.AddRange(allMemberProjectRoles);

                /*
                    For Global Projects:
                    1. Add to result custom MemberProjectRoles to each Global Projects(not Private) with all members (from db),
                    2. but dont add MemberProjectRoles with members from DB to result if it exist yet.
                */

                var addGlobalProjectsRoles = AddGlobalProjectsRoles(memberProjectRoleView);

                var addGlobalProjectsRolesView = addGlobalProjectsRoles.Select(x => x.GetViewWithGlobalProjects(Mapper, _avatarService.GetUrlIcon(x.Member.Id)));

                memberProjectRoleView.AddRange(addGlobalProjectsRolesView);

                return memberProjectRoleView;
            }

            #endregion

            #region Constrain for Manager


            var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

            #region Get All projects ids where this member(not this user) has "manager role" at projects.

            var targetedProjectIds = allMemberProjectRoles
                .Where(x => x.RoleId == managerRoleId && x.MemberId == BaseMemberImpersonated.Id)
                .Select(x => x.ProjectId)
                .ToArray();

            allMemberProjectRoles = allMemberProjectRoles.Where(x => targetedProjectIds.Contains(x.ProjectId)).ToList();

            #endregion Get All projects ids where this member(not this user) has "manager role" at projects.

            var isManager = targetedProjectIds.Length > 0;

            if (isManager)
            {
                memberProjectRoleView.AddRange(allMemberProjectRoles);

                /*
                     For Global Projects:
                     1. Add to result custom MemberProjectRoles to each Global Projects(not Private) with all members (from db),
                     2. but dont add MemberProjectRoles with members from DB to result if it exist yet.
                */

                var addGlobalProjectsRoles = AddGlobalProjectsRoles(memberProjectRoleView).Select(x => x.GetViewWithGlobalProjects(Mapper, _avatarService.GetUrlIcon(x.Member.Id)));
                    memberProjectRoleView.AddRange(addGlobalProjectsRoles);

                return memberProjectRoleView;
            }

            #endregion

            #region Constrain for user

            var resultForMember = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(p => p.MemberId == BaseMemberImpersonated.Id && p.Project.IsActive)
                .Select(x => x.GetView(Mapper, _avatarService.GetUrlIcon(x.MemberId)))
                .ToList();

            // Get Global Projects. Add members to Global Projects. Add to result Global Projects.
            resultForMember.AddRange(AddGlobalProjectsRoles(resultForMember).Select(x => x.GetViewWithGlobalProjects(Mapper, _avatarService.GetUrlIcon(x.Member.Id))));

            return resultForMember;

            #endregion
        }

        private List<MemberProjectRole> AddGlobalProjectsRoles(List<MemberProjectRoleView> memberProjRole)
        {
            var customMemberProjectRole = new List<MemberProjectRole>();

            var globalProjects = Uow.ProjectRepository.LinkedCacheGetList().Where(p => !p.IsPrivate);

            if (globalProjects.Any())
            {
                var members = Uow.MemberRepository.LinkedCacheGetList().ToList();

                // All not assigned users/members must be have "team member" role in public Projects!
                var getMemberRole = Uow.ProjectRoleRepository.GetMemberRole();

                #region Add each member to global projects!

                foreach (var project in globalProjects)
                {
                    foreach (var member in members)
                    {
                        if (!memberProjRole.Any(x => x.MemberId == member.Id && x.ProjectId == project.Id))
                        {
                            customMemberProjectRole.Add(new MemberProjectRole
                            {
                                Member = member,
                                Project = project,
                                Role = getMemberRole
                            });
                        }
                    }
                }

                #endregion
            }

            return customMemberProjectRole;
        }

        public IEnumerable<ProjectRoleView> GetProjectRoles()
        {
            var projectRole = Uow.ProjectRoleRepository.LinkedCacheGetList();

            return projectRole.Select(x => x.GetView(Mapper));
        }

        public MemberProjectRoleView GetById(int id)
        {
            var memberProjRole = Uow.MemberProjectRoleRepository.LinkedCacheGetById(id);

            if (memberProjRole == null)
            {
                throw new CoralTimeEntityNotFoundException($"MemberProjectRole with id = {id} not found.");
            }

            var memberProjRoleView = memberProjRole.GetView(Mapper, _avatarService.GetUrlIcon(memberProjRole.MemberId));

            return memberProjRoleView;
        }

        public IEnumerable<MemberView> GetNotAssignMembersAtProjByProjectId(int projectId)
        {
            if (!Uow.ProjectRepository.LinkedCacheGetById(projectId).IsPrivate)
            {
                return Enumerable.Empty<MemberView>();
            }

            var membersNotAssignProjectByProjId = Uow.MemberRepository.LinkedCacheGetList()
                .Where(member => member.MemberProjectRoles.All(mpr => mpr.ProjectId != projectId)); // for adequate count add in condition: && x.User.IsActive

            if (membersNotAssignProjectByProjId == null)
            {
                throw new CoralTimeEntityNotFoundException($"MemberProjectRole with ProjectId = {projectId} not found.");
            }

            var membersNotAssigtProjectView = membersNotAssignProjectByProjId.Select(x => x.GetView(Mapper, _avatarService.GetUrlIcon(x.Id))).ToList();

            return membersNotAssigtProjectView;
        }

        public IEnumerable<ProjectView> GetNotAssignMembersAtProjByMemberId(int memberId)
        {
            var projsWithNotAssignMembersByMembId = Uow.ProjectRepository.LinkedCacheGetList()
                .Where(project => project.MemberProjectRoles.All(mpr => mpr.MemberId != memberId)); // for adequate count add in condition: && project.IsPrivate && project.IsActive
            
            if (projsWithNotAssignMembersByMembId == null)
            {
                throw new CoralTimeEntityNotFoundException($"MemberProjectRole with MemberId = {memberId} not found.");
            }

            var memberProjRoleView = projsWithNotAssignMembersByMembId.Select(x => x.GetView(Mapper));
            return memberProjRoleView;
        }

        public MemberProjectRoleView Create(MemberProjectRoleView memberProjectRoleView)
        {
            var memberProjectRole = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .FirstOrDefault(r => r.ProjectId == memberProjectRoleView.ProjectId && r.MemberId == memberProjectRoleView.MemberId);

            if (memberProjectRole != null)
            {
                throw new CoralTimeAlreadyExistsException($"Project role with projectId = {memberProjectRoleView.ProjectId} and memberId = {memberProjectRoleView.MemberId} already exist");
            }

            var hasAccessAsManager = HasAccessAsManager(BaseMemberImpersonated, memberProjectRoleView);

            if (hasAccessAsManager)
            {
                memberProjectRole = new MemberProjectRole
                {
                    MemberId = memberProjectRoleView.MemberId,
                    ProjectId = memberProjectRoleView.ProjectId,
                    RoleId = memberProjectRoleView.RoleId
                };

                Uow.MemberProjectRoleRepository.Insert(memberProjectRole);
                Uow.Save();
                Uow.MemberProjectRoleRepository.LinkedCacheClear();

                UpdateIsManager(memberProjectRoleView.MemberId);

                var memberProjectRoleByIdResult = Uow.MemberProjectRoleRepository.LinkedCacheGetById(memberProjectRole.Id);
                
                var memberProjectRoleViewResult = memberProjectRoleByIdResult.GetView(Mapper, _avatarService.GetUrlIcon(memberProjectRole.MemberId));
                 
                return memberProjectRoleViewResult;
            }

            throw new CoralTimeForbiddenException($"Member with id = {BaseMemberImpersonated.Id} is not allowed to create MemberProjectRole on project with id = {memberProjectRoleView.ProjectId} and role with id = {memberProjectRoleView.RoleId}");
        }

        public MemberProjectRoleView Update(MemberProjectRoleView projectRole)
        {
            if (BaseMemberImpersonated == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with userName = {BaseMemberImpersonated.User.UserName} not found.");
            }

            if (!BaseMemberImpersonated.User.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"Member with userName = {BaseMemberImpersonated.User.UserName} is not active.");
            }

            var memberProjectRoleById = Uow.MemberProjectRoleRepository.GetById((int)projectRole.Id);
            if (memberProjectRoleById == null)
            {
                throw new CoralTimeEntityNotFoundException($"ProjectRole with id = {projectRole.Id} not found.");
            }

            var hasAccessAsManager = HasAccessAsManager(BaseMemberImpersonated, projectRole);

            if (hasAccessAsManager)
            {
                memberProjectRoleById.RoleId = projectRole.RoleId;

                Uow.MemberProjectRoleRepository.Update(memberProjectRoleById);
                Uow.Save();
                Uow.MemberProjectRoleRepository.LinkedCacheClear();

                UpdateIsManager(memberProjectRoleById.MemberId);

                var memberProjectRoleByIdResult = Uow.MemberProjectRoleRepository.LinkedCacheGetById((int)projectRole.Id);

                var memberProjectRoleViewResult = memberProjectRoleByIdResult.GetView(Mapper, _avatarService.GetUrlIcon(memberProjectRoleByIdResult.MemberId));
                
                return memberProjectRoleViewResult;
            }

            throw new CoralTimeForbiddenException($"Member with id = {BaseMemberImpersonated.Id} is not allowed to update projectRole on project with id = {projectRole.ProjectId} and role with id = {projectRole.RoleId}");
        }

        public MemberProjectRoleView Patch(MemberProjectRoleView projectRole)
        {
            var memberProjectRoleById = Uow.MemberProjectRoleRepository.GetById(projectRole.Id);
            if (memberProjectRoleById == null)
            {
                throw new CoralTimeEntityNotFoundException($"ProjectRole with id = {projectRole.Id} not found");
            }

            var hasAccessAsManager = HasAccessAsManager(BaseMemberImpersonated, projectRole);

            if (hasAccessAsManager)
            {
                memberProjectRoleById.RoleId = projectRole.RoleId;

                Uow.MemberProjectRoleRepository.Update(memberProjectRoleById);
                Uow.Save();
                Uow.MemberProjectRoleRepository.LinkedCacheClear();

                UpdateIsManager(memberProjectRoleById.MemberId);

                var memberProjectRoleByIdResult = Uow.MemberProjectRoleRepository.LinkedCacheGetById(projectRole.Id);

                var memberProjectRoleVIewResult = memberProjectRoleByIdResult.GetViewWithGlobalProjects(Mapper, _avatarService.GetUrlIcon(memberProjectRoleByIdResult.MemberId));
                return memberProjectRoleVIewResult;
            }

            throw new CoralTimeForbiddenException($"Member with id = {BaseMemberImpersonated.Id} is not allowed to patch projectRole on project with id = {projectRole.ProjectId} and role with id = {projectRole.RoleId}");
        }

        public void Delete(int id)
        {
            var projectRole = Uow.MemberProjectRoleRepository.GetById(id);
            if (projectRole == null)
            {
                throw new CoralTimeEntityNotFoundException($"ProjectRole with id {id} not found");
            }

            var hasAccessAsManager = HasAccessAsManager(BaseMemberImpersonated, new MemberProjectRoleView
            {
                ProjectId = projectRole.ProjectId,
                RoleId = projectRole.RoleId
            });

            if (hasAccessAsManager)
            {
                Uow.MemberProjectRoleRepository.Delete(id);
                Uow.Save();
                Uow.MemberProjectRoleRepository.LinkedCacheClear();
                UpdateIsManager(projectRole.MemberId);
            }
            else
            {
                throw new CoralTimeForbiddenException($"Member with id {BaseMemberImpersonated.Id} is not allowed to delete projectRole on project with id {projectRole.ProjectId} and role with id {projectRole.RoleId}.");
            }
        }

        public bool FixAllManagerRoles()
        {
            var members = Uow.MemberRepository.LinkedCacheGetList().Select(x => x.Id).ToList();

            foreach (var memberId in members)
            {
                UpdateIsManager(memberId);
            }

            return true;
        }

        private bool HasAccessAsManager(Member member, MemberProjectRoleView projectRole)
        {
            var managesAll = Authorize(member, Constants.PolicyManagesAllProjects);
            if (managesAll)
            {
                return true;
            }

            var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();
            var memberRoleId = Uow.ProjectRoleRepository.GetMemberRoleId();

            //check if current user is manager on selected project and is trying to assign team member
            var hasAccessAsManager = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                                         .Any(r => r.ProjectId == projectRole.ProjectId && r.MemberId == member.Id && r.RoleId == managerRoleId) && projectRole.RoleId == memberRoleId;

            return hasAccessAsManager;
        }

        private void UpdateIsManager(int memberId)
        {
            //No longer necessary. Can be removed in a future release.
        }

        private int CountManagerRoles(int memberId) => Uow.MemberProjectRoleRepository.LinkedCacheGetList().Count(x => x.MemberId == memberId && x.RoleId == Uow.ProjectRoleRepository.GetManagerRoleId());
    }
}