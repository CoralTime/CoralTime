using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using System.Text.Json;

namespace CoralTime.BL.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<ProjectView> TimeTrackerAllProjects();

        IEnumerable<ManagerProjectsView> ManageProjectsOfManager();

        ProjectView GetById(int id);

        ProjectView Update(int projectId, JsonElement project);

        ProjectView Create(ProjectView newProject);

        ProjectView Patch(int projectId, JsonElement project);

        IEnumerable<MemberView> GetMembers(int projectId);

        IEnumerable<ProjectNameView> GetProjectsNames();

        bool Delete(int id);
    }
}