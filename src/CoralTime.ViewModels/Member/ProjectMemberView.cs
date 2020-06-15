using CoralTime.ViewModels.Member.MemberImage;

namespace CoralTime.ViewModels.Member
{
    public class ProjectMembersView : IMemberImageIconView
    {
        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public string UrlIcon { get; set; }
    }
}