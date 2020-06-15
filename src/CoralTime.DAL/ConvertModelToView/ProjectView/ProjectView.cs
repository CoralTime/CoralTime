using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CoralTime.DAL.Models;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ViewModels.Projects.ProjectView GetView(this Project project, IMapper mapper)
        {
            var projectByManagerView = mapper.Map<Project, ViewModels.Projects.ProjectView>(project);

            return projectByManagerView;
        }

        public static ViewModels.Projects.ProjectView GetViewManageProjectsOfManager(this Project project, IMapper mapper, int allMembersActiveCount = 0, List<TaskType> globalTasks = null)
        {
            var projectByManagerView = mapper.Map<Project, ViewModels.Projects.ProjectView>(project);

            projectByManagerView.MembersCount = project.IsPrivate
                ? project.MemberProjectRoles.Select(r => r.Member.User).Count(x => x.IsActive)
                : allMembersActiveCount;
            projectByManagerView.TasksCount = CustomTasksWithGlobalTasksCount(project, globalTasks);

            return projectByManagerView;
        }

        public static ViewModels.Projects.ProjectView GetViewTimeTrackerAllProjects(this Project project, IMapper mapper, int allMembersCount = 0, string userName = null, List<TaskType> globalTasks = null)
        {
            var projectAdminView = mapper.Map<Project, ViewModels.Projects.ProjectView>(project);

            // TODO compare MembersCount from ProjectsOfManager.
            projectAdminView.MembersCount = project.IsPrivate
                ? project.MemberProjectRoles.Select(r => r.Member.User).Count()
                : allMembersCount;
            projectAdminView.TasksCount = CustomTasksWithGlobalTasksCount(project, globalTasks);
            projectAdminView.IsCurrentUserOnProject = project.MemberProjectRoles.Any(r => r.Member.User.UserName == userName) || !project.IsPrivate;

            return projectAdminView;
        }

        public static ViewModels.Projects.ProjectView GetViewProjectFromProjMemberRole(this Project project, IMapper mapper, string userName = null, List<TaskType> globalTasks = null)
        {
            var projectManagerMemberView = mapper.Map<Project, ViewModels.Projects.ProjectView>(project);

            projectManagerMemberView.IsCurrentUserOnProject = project.MemberProjectRoles == null
                ? !project.IsPrivate
                : project.MemberProjectRoles.Any(r => r.Member.User.UserName == userName) || !project.IsPrivate;
            projectManagerMemberView.MembersCount = project.MemberProjectRoles?.Count(r => r.ProjectId == project.Id && r.Member.User.IsActive) ?? 0;
            projectManagerMemberView.TasksCount = project.TaskTypes?.Count(p => (p.ProjectId == project.Id || p.ProjectId == null) && p.IsActive) ?? 0;

            return projectManagerMemberView;
        }

        public static ViewModels.Projects.ProjectView GetViewProjectByManager(this Project project, IMapper mapper, string userName = null, List<TaskType> globalTasks = null)
        {
            var projectByManagerView = mapper.Map<Project, ViewModels.Projects.ProjectView>(project);

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