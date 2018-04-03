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

        // Tab "Projects - Grid". Return projects for manager where it has "manager" role only!
        public IEnumerable<ProjectView> ManageProjectsOfManager()
        {
            var user = Uow.UserRepository.LinkedCacheGetByName(ImpersonatedUserName);
            if (user == null)
            {
                throw new CoralTimeEntityNotFoundException($"User with userName {ImpersonatedUserName} not found.");
            }

            var getAllProjects = Uow.ProjectRepository.LinkedCacheGetList();

            var getGlobalTasks = Uow.TaskTypeRepository.LinkedCacheGetList()
                .Where(j => j.ProjectId == null && j.IsActive)
                .ToList();

            #region Constrain for admin: return all projects. Tab "Projects - Grid".

            if (user.IsAdmin)
            {
                return getAllProjects.Select(p => p.GetViewManageProjectsOfManager(Mapper, GetCacheMembersActiveCount(), getGlobalTasks));
            }

            #endregion Constrain for admin: return all projects. Tab "Projects - Grid".

            #region Constrain for manager: return all projects for Member where it assing at Manager. Tab "Projects - Grid" available (Front-end check) if return something.

            if (user.IsManager)
            {
                var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();
                var memberId = Uow.MemberRepository.GetQueryByUserName(user.UserName).Id;

                // Get All projects ids where member has manager role at this project.
                var targetedProjects = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(x => x.RoleId == managerRoleId && x.MemberId == memberId)
                .Select(x => x.Project)
                .ToList();

                return targetedProjects.Select(p => p.GetViewManageProjectsOfManager(Mapper, GetCacheMembersActiveCount(), getGlobalTasks));
            }

            #endregion Constrain for manager: return all projects for Member where it assing at Manager. Tab "Projects - Grid" available (Front-end check) if return something.

            // If user is not manager at project. return empty list. Tab "Projects - Grid" not be available (Front-end check).
            return new List<ProjectView>();
        }

        // Tab "TimeTracker -> All Projects"
        public IEnumerable<ProjectView> TimeTrackerAllProjects()
        {
            var user = Uow.UserRepository.LinkedCacheGetByName(ImpersonatedUserName);
            if (user == null)
            {
                throw new CoralTimeEntityNotFoundException("User with userName " + $"{ImpersonatedUserName} not found");
            }

            var getAllProjects = Uow.ProjectRepository.LinkedCacheGetList();

            var getGlobalTasks = Uow.TaskTypeRepository
                .LinkedCacheGetList()
                .Where(j => j.ProjectId == null && j.IsActive)
                .ToList();

            #region Constrain for: admin: return all projects. Tab "TimeTracker -> All Projects".

            if (user.IsAdmin)
            {
                return getAllProjects.Select(p => p.GetViewTimeTrackerAllProjects(Mapper, GetCacheMembersActiveCount(), ImpersonatedUserName, getGlobalTasks));
            }

            #endregion Constrain for: admin: return all projects. Tab "TimeTracker -> All Projects".

            #region Constrain for: manager, member.

            var getGlobalProjects = getAllProjects
                .Where(x => !x.IsPrivate)
                .Select(z => z.GetViewProjectFromProjMemberRole(Mapper, ImpersonatedUserName, getGlobalTasks));

            var memberId = Uow.MemberRepository.LinkedCacheGetByName(user.UserName).Id;

            // Get All projs for Member.
            var getProjectsForMember = GetProjectsForMember(memberId, ImpersonatedUserName);

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

            #endregion Constrain for: manager, member.
        }

        public ProjectView GetById(int id)
        {
            var projectById = Uow.ProjectRepository.LinkedCacheGetById(id);

            if (projectById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id = {id} not found.");
            }

            return projectById.GetViewTimeTrackerAllProjects(Mapper, GetCacheMembersActiveCount(), ImpersonatedUserName);
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

        public ProjectView Create(ProjectView projectData)
        {
            var user = Uow.UserRepository.LinkedCacheGetByName(CurrentUserName);
            if (user == null)
            {
                throw new CoralTimeEntityNotFoundException($"User with userName {CurrentUserName} not found");
            }

            // TODO rewrite!
            projectData.Name = projectData.Name == null
                ? projectData.Name
                : projectData.Name.Trim();

            var isNameUnique = Uow.ProjectRepository.LinkedCacheGetByName(projectData.Name) == null;

            var project = Mapper.Map<ProjectView, Project>(projectData);
            project.IsActive = true;
            project.IsPrivate = true;

            BLHelpers.CheckProjectsErrors(project, isNameUnique);

            try
            {
                Uow.ProjectRepository.Insert(project);
                Uow.Save();

                Uow.ProjectRepository.LinkedCacheClear();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while creating project", e);
            }

            var projectById = Uow.ProjectRepository.LinkedCacheGetById(project.Id);

            return projectById.GetViewTimeTrackerAllProjects(Mapper, GetCacheMembersActiveCount());
        }

        public ProjectView Update(dynamic projectView)
        {
            var projectById = Uow.ProjectRepository.GetById((int)projectView.Id);

            if (projectById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id = {projectView.Id} not found.");
            }

            return CommonLogicForPatchUpdateMethods(projectView, CurrentUserName, projectById);
        }

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

            return CommonLogicForPatchUpdateMethods(projectView, CurrentUserName, projectById);
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

            try
            {
                Uow.ProjectRepository.Delete(projectById.Id);
                Uow.Save();

                Uow.ProjectRepository.LinkedCacheClear();

                return true;
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while deleting timeEntryType", e);
            }
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

        private ProjectView CommonLogicForPatchUpdateMethods(dynamic projectView, string userName, Project projectById)
        {
            var user = Uow.UserRepository.LinkedCacheGetByName(userName);
            if (user == null)
            {
                throw new CoralTimeEntityNotFoundException($"User with userName {userName} not found.");
            }

            var newProjectName = (string)projectView.name;

            var isNameUnique = Uow.ProjectRepository.LinkedCacheGetByName(newProjectName) == null || projectById.Name == newProjectName;

            if (projectView.isActive != null && !(bool)projectView.isActive)
            {
                var timeEntries = Uow.TimeEntryRepository.GetQueryWithIncludes()
                    .Where(t => t.ProjectId == projectById.Id && t.Date.Date == DateTime.Now.Date)
                    .ToList();

                timeEntries.ForEach(t => t.StopTimer());
            }

            UpdateService<Project>.UpdateObject(projectView, projectById);

            BLHelpers.CheckProjectsErrors(projectById, isNameUnique);

            try
            {
                Uow.ProjectRepository.Update(projectById);
                Uow.Save();
                Uow.ProjectRepository.LinkedCacheClear();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating project", e);
            }

            var projectByIdResult = Uow.ProjectRepository.LinkedCacheGetById(projectById.Id);

            return projectByIdResult.GetViewTimeTrackerAllProjects(Mapper, GetCacheMembersActiveCount());
        }

        public IEnumerable<ProjectNameView> GetProjectsNames()
        {
            var projectNames = Uow.ProjectRepository.LinkedCacheGetList()
                .Select(x => x.GetViewProjectName(Mapper));

            return projectNames;
        }

        #endregion Added methods.

        public int GetCacheMembersActiveCount()
        {
            return Uow.MemberRepository.LinkedCacheGetList().Count(x => x.User.IsActive);
        }
    }
}