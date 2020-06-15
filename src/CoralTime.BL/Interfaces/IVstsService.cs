using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.TimeEntries;
using CoralTime.ViewModels.Vsts;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace CoralTime.BL.Interfaces
{
    public interface IVstsService
    {
        List<VstsTask> GetTasksByProject(string projectId);

        int? GetProjectIdByVstsProjectId(string projectId);

        Member GetVstsMember(string token);

        bool SaveTimeEntry(TimeEntryView vstsTimeEnry, Member member);

        JwtSecurityToken ValidateToken(string issuedToken);

        VstsSetup GetVstsSetupInfo(VstsSetup vstsSetup);

        List<VstsTimeEntry> GetTimeEntriesByWorkItemId(string projectId, string workItemId);

        IQueryable<VstsProjectIntegrationView> Get();

        VstsProjectIntegrationView Create(VstsProjectIntegrationView vstsProjectIntegrationView);

        VstsProjectIntegrationView Update(VstsProjectIntegrationView vstsProjectIntegrationView);

        bool Delete(int id);
        IEnumerable<VstsMemberView> GetMembersByProjectId(int id);
    }
}