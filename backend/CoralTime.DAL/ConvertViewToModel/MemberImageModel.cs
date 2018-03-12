using CoralTime.DAL.Models;

namespace CoralTime.DAL.ConvertViewToModel
{
    public static partial class ConvertViewToModel
    {
        public static MemberImage UpdateProperties(this MemberImage memberImage, MemberImage newMemberImage)
        {
            memberImage.MemberId = newMemberImage.MemberId;
            memberImage.FileNameImage = newMemberImage.FileNameImage;
            memberImage.ByteArrayAvatar = newMemberImage.ByteArrayAvatar;
            memberImage.ByteArrayIcon = newMemberImage.ByteArrayIcon;

            return memberImage;
        }
    }
}
