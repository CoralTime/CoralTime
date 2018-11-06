using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Vsts;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace CoralTime.BL.Interfaces
{
    public interface IVstsService
    {
        List<VstsTask> GetTasksByProject(string projectName);

        int? GetProjectIdByProjectName(string projectName);

        Member GetVstsMember(string token, string id, string userName);

        bool SaveTimeEntry(VstsTimeEntry vstsTimeEnry);

        JwtSecurityToken ValidateToken(string issuedToken);

        VstsSetup GetVstsSetupInfo(VstsSetup vstsSetup);
    }
}