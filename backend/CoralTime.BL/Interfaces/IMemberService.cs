using CoralTime.DAL.Models;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces
{
    public interface IMemberService
    {
        IEnumerable<MemberView> GetAllMembers(string userName);

        Member GetById(int id);

        IEnumerable<ProjectView> GetTimeTrackerAllProjects(int memberId);

        Task<Member> CreateNewUser(MemberView member);

        Task<MemberView> Update(string userName, MemberView memberView);

        #region Password.

        Task ChangePassword(MemberChangePasswordView member);

        Task ResetPassword(int id);

        Task<ChangePasswordResultView> ChangePasswordByTokenAsync(MemberChangePasswordByTokenView model);

        #endregion

        #region Emails.

        Task SentInvitationEmailAsync(MemberView member, string baseUrl);

        Task SentUpdateAccountEmailAsync(MemberView member, string baseUrl);

        Task<PasswordForgotEmailResultView> SentForgotEmailAsync(string email, string url);

        Task<CheckForgotPasswordTokenResultView> CheckForgotPasswordTokenAsync(string token);

        Task ChangeEmailByUserAsync(Member member, string newEmail);

        #endregion

        #region Claims & Token.

        void UpdateUsersClaims();

        void UpdateUserClaims(int memberId);

        #endregion

        #region Other Methods.

        IEnumerable<Member> GetAllMembersCommon(string userName);

        IEnumerable<Project> GetProjects(int memberId);

        #endregion
    }
}