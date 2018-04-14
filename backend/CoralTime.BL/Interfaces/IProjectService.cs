using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<ProjectView> TimeTrackerAllProjects();

        IEnumerable<ProjectView> ManageProjectsOfManager();

        ProjectView GetById(int id);

        ProjectView Update(dynamic project);

        ProjectView Create(ProjectView newProject);

        ProjectView Patch(dynamic project);

        IEnumerable<MemberView> GetMembers(int projectId);

        IEnumerable<ProjectNameView> GetProjectsNames();

        bool Delete(int id);
    }
}