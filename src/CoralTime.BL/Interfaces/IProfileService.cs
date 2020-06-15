using CoralTime.ViewModels.DateFormat;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using System.Collections.Generic;
using CoralTime.ViewModels.Projects.Profile;

namespace CoralTime.BL.Interfaces
{
    public interface IProfileService
    {
        List<ProfileProjectView> GetMemberProjects();

        DateConvert[] GetDateFormats();

        IEnumerable<ProjectMembersView> GetProjectMembers(int projectId);

        //MemberView PatchNotifications(MemberNotificationView memberNotificationView);

        MemberView PatchPreferences(MemberPreferencesView memberPreferencesView);

        MemberView PatchPersonalInfo(MemberPersonalInfoView memberPreferencesView);
    }
}