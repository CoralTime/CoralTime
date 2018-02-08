using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;

namespace CoralTime.BL.Interfaces
{
    public interface IAvatarService
    {
        string GetAvatarUrl(int memberId);
        string GetIconUrl(int memberId);
        MemberAvatarView SetUpdateMemberAvatar(IFormFile uploadedFile);
        MemberAvatarView GetAvatar(int memberId);
        MemberAvatarView GetIcon(int memberId);
        void AddIconUrlinMembeView(MemberView memberView);
    }
}