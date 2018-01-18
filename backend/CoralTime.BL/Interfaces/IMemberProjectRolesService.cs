using CoralTime.ViewModels.MemberProjectRoles;
using CoralTime.ViewModels.ProjectRole;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using CoralTime.ViewModels.Member;

namespace CoralTime.BL.Interfaces
{
    public interface IMemberProjectRolesService
    {
        IEnumerable<ProjectRoleView> GetProjectRoles();

        IEnumerable<MemberProjectRoleView> GetAllProjectRoles(string userName);

        MemberProjectRoleView GetById(int id);
            
        MemberProjectRoleView Create(string userName, MemberProjectRoleView projectRole);

        MemberProjectRoleView Update(string userName, dynamic projectRole);

        MemberProjectRoleView Patch(string userName, MemberProjectRoleView projectRole);

        void Delete(string userName, int id);

        //TODO DELETE???
        IEnumerable<ProjectView> GetAllProjectsByManager(string userName);
        
        bool FixAllManagerRoles();

        IEnumerable<MemberView> GetNotAssignMembersAtProjByProjectId(int projectId);

        IEnumerable<ProjectView> GetNotAssignMembersAtProjByMemberId(int memberId);
    }
}