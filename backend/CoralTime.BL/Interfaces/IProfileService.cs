using CoralTime.ViewModels.DateFormat;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Member.MemberNotificationView;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces
{
    public interface IProfileService
    {
        List<ProfileProjectView> GetMemberProjects();

        MemberAvatarView GetMemberAvatar(int memberId);

        MemberAvatarView SetUpdateMemberAvatar(IFormFile uploadedFile);

        MemberAvatarView GetMemberIcon(int memberId);

        DateConvert[] GetDateFormats();

        List<ProfileProjectMemberView> GetProjectMembers(int projectId);

        MemberView PatchNotifications(MemberNotificationView memberNotificationView);

        MemberView PatchPreferences(MemberPreferencesView memberPreferencesView);

        MemberView PatchPersonalInfo(MemberPersonalInfoView memberPreferencesView);
    }
}