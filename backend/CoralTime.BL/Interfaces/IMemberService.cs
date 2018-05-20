using CoralTime.DAL.Models;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoralTime.BL.Interfaces
{
    public interface IMemberService
    {
        IEnumerable<MemberView> GetAllMembers();

        MemberView GetById(int id);

        IEnumerable<ProjectView> GetTimeTrackerAllProjects(int memberId);

        Task<MemberView> CreateNewUser(MemberView member, string baseUrl);

        Task<MemberView> Update(MemberView memberView, string baseUrl);

        #region Password.

        Task ChangePassword(MemberChangePasswordView member);

        Task ResetPassword(int id);

        Task<ChangePasswordResultView> ChangePasswordByTokenAsync(MemberChangePasswordByTokenView model);

        #endregion

        #region Emails.

        Task<PasswordForgotEmailResultView> SentForgotEmailAsync(string email, string serverUrl);

        Task<CheckForgotPasswordTokenResultView> CheckForgotPasswordTokenAsync(string token);

        Task ChangeEmailByUserAsync(Member member, string newEmail);

        #endregion

        #region Claims & Token.

        void UpdateUsersClaims();

        void UpdateUserClaims(int memberId);

        #endregion

        #region Other Methods.

        IEnumerable<Project> GetProjects(int memberId);

        #endregion
    }
}