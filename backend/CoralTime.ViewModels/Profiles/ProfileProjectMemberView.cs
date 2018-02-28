using CoralTime.ViewModels.Interfaces;

namespace CoralTime.ViewModels.Profiles
{
    public class ProfileProjectMemberView : IAvatarViewModel
    {
        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public string UrlIcon { get; set; }
    }
}