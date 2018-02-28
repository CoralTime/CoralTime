using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;

namespace CoralTime.BL.Interfaces
{
    public interface IAvatarService
    {
        string GetUrlAvatar(int memberId);

        string GetUrlIcon(int memberId);

        MemberAvatarView SetUpdateMemberAvatar(IFormFile uploadedFile);

        void SaveIconsAndAvatarsToStaticFiles();
    }
}