using CoralTime.ViewModels.MemberProjectRoles;
using CoralTime.ViewModels.ProjectRole;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using CoralTime.ViewModels.Member;

namespace CoralTime.BL.Interfaces
{
    public interface IMemberProjectRoleService
    {
        IEnumerable<ProjectRoleView> GetProjectRoles();

        IEnumerable<MemberProjectRoleView> GetAllProjectRoles();

        MemberProjectRoleView GetById(int id);
            
        MemberProjectRoleView Create(MemberProjectRoleView projectRole);

        MemberProjectRoleView Update(dynamic projectRole);

        MemberProjectRoleView Patch(MemberProjectRoleView projectRole);

        void Delete(int id);

        //TODO DELETE???
        IEnumerable<ProjectView> GetAllProjectsByManager();
        
        bool FixAllManagerRoles();

        IEnumerable<MemberView> GetNotAssignMembersAtProjByProjectId(int projectId);

        IEnumerable<ProjectView> GetNotAssignMembersAtProjByMemberId(int memberId);
    }
}