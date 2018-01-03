using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;

namespace CoralTime.BL.ServicesInterfaces
{
    public interface IProjectService
    {
        IEnumerable<ProjectView> TimeTrackerAllProjects(string userName);

        IEnumerable<ProjectView> ManageProjectsOfManager(string userName);

        ProjectView GetById(int id, string userName);

        ProjectView Update(dynamic project, string userName);

        ProjectView Create(ProjectView newProject, string userName);

        ProjectView Patch(dynamic project, string userName);

        IEnumerable<MemberView> GetMembers(int projectId);

        IEnumerable<ProjectNameView> GetProjectsNames();

        bool Delete(int id);
    }
}