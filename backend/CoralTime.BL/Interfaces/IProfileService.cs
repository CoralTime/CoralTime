using CoralTime.Common.Models;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Member.MemberNotificationView;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoralTime.DAL.Models;

namespace CoralTime.BL.Interfaces
{
    public interface IProfileService
    {
        List<ProfileProjectView> GetMemberProjects(string userName);

        MemberAvatarView GetMemberAvatar(string userName, int memberId);

        MemberAvatarView SetUpdateMemberAvatar(IFormFile uploadedFile, string userName);

        MemberAvatarView GetMemberIcon(string userName, int memberId);

        DateConvert[] GetDateFormats();

        List<ProfileProjectMemberView> GetProjectMembers(int projectId, string userName);

        MemberView PatchNotifications(string userName, MemberNotificationView memberNotificationView);

        MemberView PatchPreferences(string userName, MemberPreferencesView memberPreferencesView);

        MemberView PatchPersonalInfo(string userName, MemberPersonalInfoView memberPreferencesView);
    }
}