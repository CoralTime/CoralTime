using CoralTime.DAL.Models;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ProjectView GetView(this Project project, IMapper _mapper)
        {
            var projectByManagerView = _mapper.Map<Project, ProjectView>(project);

            return projectByManagerView;
        }

        public static ProjectView GetViewManageProjectsOfManager(this Project project, IMapper _mapper, int allMembersActiveCount = 0, List<TaskType> globalTasks = null)
        {
            var projectByManagerView = _mapper.Map<Project, ProjectView>(project);

            projectByManagerView.MembersCount = project.IsPrivate
                ? project.MemberProjectRoles.Select(r => r.Member.User).Count(x => x.IsActive)
                : allMembersActiveCount;
            projectByManagerView.TasksCount = CustomTasksWithGlobalTasksCount(project, globalTasks);

            return projectByManagerView;
        }

        public static ProjectView GetViewTimeTrackerAllProjects(this Project project, IMapper _mapper, int allMembersCount = 0, string userName = null, List<TaskType> globalTasks = null)
        {
            var projectAdminView = _mapper.Map<Project, ProjectView>(project);

            // TODO compare MembersCount from ProjectsOfManager.
            projectAdminView.MembersCount = project.IsPrivate
                ? project.MemberProjectRoles.Select(r => r.Member.User).Count()
                : allMembersCount;
            projectAdminView.TasksCount = CustomTasksWithGlobalTasksCount(project, globalTasks);
            projectAdminView.IsCurrentUserOnProject = project.MemberProjectRoles.Any(r => r.Member.User.UserName == userName) || !project.IsPrivate;

            return projectAdminView;
        }

        public static ProjectView GetViewProjectFromProjMemberRole(this Project project, IMapper _mapper, string userName = null, List<TaskType> globalTasks = null)
        {
            var projectManagerMemberView = _mapper.Map<Project, ProjectView>(project);

            projectManagerMemberView.IsCurrentUserOnProject = project.MemberProjectRoles == null
                ? !project.IsPrivate
                : project.MemberProjectRoles.Any(r => r.Member.User.UserName == userName) || !project.IsPrivate;
            projectManagerMemberView.MembersCount = project.MemberProjectRoles?.Count(r => r.ProjectId == project.Id && r.Member.User.IsActive) ?? 0;
            projectManagerMemberView.TasksCount = project.TaskTypes?.Count(p => (p.ProjectId == project.Id || p.ProjectId == null) && p.IsActive) ?? 0;

            return projectManagerMemberView;
        }

        public static ProjectView GetViewProjectByManager(this Project project, IMapper _mapper, string userName = null, List<TaskType> globalTasks = null)
        {
            var projectByManagerView = _mapper.Map<Project, ProjectView>(project);

            projectByManagerView.TasksCount = CustomTasksWithGlobalTasksCount(project, globalTasks);
            projectByManagerView.MembersCount = project.MemberProjectRoles.Count(r => r.Member.User.IsActive);
            projectByManagerView.IsCurrentUserOnProject = project.MemberProjectRoles.Any(r => r.Member.User.UserName == userName) || !project.IsPrivate;

            return projectByManagerView;
        }

        #region Added methods

        // Add glob tasks; check dublicates task ("custom task" has more important priority)
        private static int CustomTasksWithGlobalTasksCount(Project project, List<TaskType> globalTasks)
        {
            var taskTypes = new List<TaskType>();

            taskTypes.AddRange(project.TaskTypes.Where(t => t.IsActive));

            if (globalTasks != null)
            {
                foreach (var task in globalTasks)
                {
                    // If list have custom task, don't add global!
                    var hasCustomTask = taskTypes.Any(z => z.Id == task.Id);
                    if (!hasCustomTask)
                    {
                        taskTypes.Add(task);
                    }
                }
            }

            return taskTypes.Count;
        }

        #endregion Added methods
    }
}