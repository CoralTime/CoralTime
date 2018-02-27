namespace CoralTime.ViewModels.Profiles
{
    public class MemberAvatarView
    {
        public int MemberId { get; set; }

        public byte[] AvatarFile { get; set; }

        public string AvatarFileName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
