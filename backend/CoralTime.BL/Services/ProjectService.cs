using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Projects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.BL.Services
{
    public class ProjectService : BaseService, IProjectService
    {
        private readonly IImageService _avatarService;

        public ProjectService(UnitOfWork uow, IMapper mapper, IImageService avatarService)
            : base(uow, mapper)
        {
            _avatarService = avatarService;
        }

        // TODO Check This feature!
        // Tab "Projects - Grid". Return projects for manager where it has "manager" role only!
        public IEnumerable<ManagerProjectsView> ManageProjectsOfManager()
        {
            // If user is not manager at project. return empty list. Tab "Projects - Grid" not be available (Front-end check).
            if (!BaseMemberImpersonated.User.IsAdmin && !BaseMemberImpersonated.User.IsManager)
            {
                return new List<ManagerProjectsView>();
            }

            var projectsView = new List<ProjectView>();

            //Constraint for admin: return all projects. Tab "Projects - Grid".
            if (BaseMemberImpersonated.User.IsAdmin)
            {
                projectsView = Uow.ProjectRepository.LinkedCacheGetList().Select(p => p.GetViewManageProjectsOfManager(Mapper, CountActiveMembers(), GetGlobalActiveTasks())).ToList();
            }
            // Constraint for manager: return all projects for Member where it assing at Manager. Tab "Projects - Grid" available (Front-end check) if return something.
            else if(!BaseMemberImpersonated.User.IsAdmin && BaseMemberImpersonated.User.IsManager)
            {
                // Get All projects ids where member has manager role at this project.
                var targetedProjects = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                    .Where(x => x.RoleId == Uow.ProjectRoleRepository.GetManagerRoleId() && x.MemberId == BaseMemberImpersonated.Id)
                    .Select(x => x.Project)
                    .ToList();

                projectsView = targetedProjects.Select(p => p.GetViewManageProjectsOfManager(Mapper, CountActiveMembers(), GetGlobalActiveTasks())).ToList();
            }

            return projectsView.Select(Mapper.Map<ProjectView, ManagerProjectsView>);
        }

        private List<TaskType> GetGlobalActiveTasks() => Uow.TaskTypeRepository.LinkedCacheGetList().Where(j => j.ProjectId == null && j.IsActive).ToList();

        // Tab "TimeTracker -> All Projects"
        public IEnumerable<ProjectView> TimeTrackerAllProjects()
        {
            var getAllProjects = Uow.ProjectRepository.LinkedCacheGetList();

            var getGlobalTasks = GetGlobalActiveTasks();

            #region Constrain for: admin: return all projects. Tab "TimeTracker -> All Projects".

            if (BaseMemberImpersonated.User.IsAdmin)
            {
                return getAllProjects.Select(p => p.GetViewTimeTrackerAllProjects(Mapper, CountActiveMembers(), BaseMemberImpersonated.User.UserName, getGlobalTasks));
            }

            #endregion

            #region Constrain for: manager, member.

            var getGlobalProjects = getAllProjects
                .Where(x => !x.IsPrivate)
                .Select(z => z.GetViewProjectFromProjMemberRole(Mapper, BaseMemberImpersonated.User.UserName, getGlobalTasks));

            // Get All projs for Member.
            var getProjectsForMember = GetProjectsForMember(BaseMemberImpersonated.Id, BaseMemberImpersonated.User.UserName);

            foreach (var project in getGlobalProjects)
            {
                // If custom project assign at user/member behavior, that don't add global project to list.
                var resProjs = getProjectsForMember.Any(z => z.Id == project.Id);
                if (!resProjs)
                {
                    getProjectsForMember.Add(project);
                }
            }

            return getProjectsForMember;

            #endregion
        }

        public ProjectView GetById(int id)
        {
            var projectById = Uow.ProjectRepository.LinkedCacheGetById(id);

            if (projectById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id = {id} not found.");
            }

            return projectById.GetViewTimeTrackerAllProjects(Mapper, CountActiveMembers(), BaseMemberImpersonated.User.UserName);
        }

        public IEnumerable<MemberView> GetMembers(int projectId)
        {
            var project = Uow.ProjectRepository.LinkedCacheGetById(projectId);
            if (project == null)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id {projectId} not found.");
            }

            var member = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(t => t.ProjectId == project.Id && t.Member.User.IsActive)
                .Select(m => m.Member);

            var memberView = member.Select(x => x.GetViewWithProjectCount(Mapper, _avatarService.GetUrlIcon(x.Id)));
            return memberView;
        }

        public ProjectView Create(dynamic projectView)
        {
            var localName = (string) projectView.name;
            var isNameUnique = Uow.ProjectRepository.LinkedCacheGetByName(localName) == null;

            var project = Mapper.Map<ProjectView, Project>(new ProjectView
            {
                ClientId = projectView.clientId,
                Color = projectView.color,
                IsActive = projectView.isActive,
                Name = projectView.name,
                ClientIsActive = projectView.clientIsActive
            });

            project.IsActive = true;
            project.IsPrivate = true;

            BLHelpers.CheckProjectsErrors(project, isNameUnique);

            Uow.ProjectRepository.Insert(project);
            Uow.Save();
            Uow.ProjectRepository.LinkedCacheClear();

            var projectById = Uow.ProjectRepository.LinkedCacheGetById(project.Id);

            return projectById.GetViewTimeTrackerAllProjects(Mapper, CountActiveMembers());
        }

        // TODO remove dynamic!
        public ProjectView Update(dynamic projectView)
        {
            var projectById = Uow.ProjectRepository.GetById((int)projectView.Id);

            if (projectById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id = {projectView.Id} not found.");
            }

            return CommonLogicForPatchUpdateMethods(projectView, projectById);
        }

        // TODO remove dynamic!
        public ProjectView Patch(dynamic projectView)
        {
            var projectById = Uow.ProjectRepository.GetById((int)projectView.Id);
            if (projectById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id = {projectView.Id} not found.");
            }

            // Don't activate project if client is Inactive.
            if (projectById.Client != null)
            {
                if (!projectById.Client.IsActive)
                {
                    throw new CoralTimeDangerException("Cannot activate project, because Client is inactive or don't have permission to edit the project.");
                }
            }

            return CommonLogicForPatchUpdateMethods(projectView, projectById);
        }

        public bool Delete(int id)
        {
            var projectById = Uow.ProjectRepository.LinkedCacheGetById(id);
            if (projectById == null || !projectById.IsActive)
            {
                {
                    throw new CoralTimeEntityNotFoundException("project with id " +
                       $"{id} not found or is not active");
                }
            }

            Uow.ProjectRepository.Delete(projectById.Id);
            Uow.Save();
            Uow.ProjectRepository.LinkedCacheClear();

            return true;
        }

        #region Added methods.

        private List<ProjectView> GetProjectsForMember(int memberId, string userName)
        {
            // Get all roles where member is assign.
            var memberProjectRoles = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(w => w.MemberId == memberId)
                .ToList();

            // Get projects from roles where member is assign.
            var projects = memberProjectRoles.Select(x => x.Project.GetViewProjectFromProjMemberRole(Mapper, userName)).ToList();

            return projects;
        }

        private ProjectView CommonLogicForPatchUpdateMethods(dynamic projectView, Project projectById)
        {
            var newProjectName = (string)projectView.name;

            var isNameUnique = Uow.ProjectRepository.LinkedCacheGetByName(newProjectName) == null || projectById.Name == newProjectName;

            if (projectView.isActive != null && !(bool)projectView.isActive)
            {
                var timeEntries = Uow.TimeEntryRepository.GetQuery()
                    .Where(t => t.ProjectId == projectById.Id && t.Date.Date == DateTime.Now.Date)
                    .ToList();

                timeEntries.ForEach(t => t.StopTimer());
            }

            UpdateService<Project>.UpdateObject(projectView, projectById);

            BLHelpers.CheckProjectsErrors(projectById, isNameUnique);

            Uow.ProjectRepository.Update(projectById);
            Uow.Save();
            Uow.ProjectRepository.LinkedCacheClear();

            var projectByIdResult = Uow.ProjectRepository.LinkedCacheGetById(projectById.Id);

            return projectByIdResult.GetViewTimeTrackerAllProjects(Mapper, CountActiveMembers());
        }

        public IEnumerable<ProjectNameView> GetProjectsNames()
        {
            var projectNames = Uow.ProjectRepository.LinkedCacheGetList()
                .Select(x => x.GetViewProjectName(Mapper));

            return projectNames;
        }

        #endregion Added methods.

        private int CountActiveMembers() => Uow.MemberRepository.LinkedCacheGetList().Count(x => x.User.IsActive);
    }
}