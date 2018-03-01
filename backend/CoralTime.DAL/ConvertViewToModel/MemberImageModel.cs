using CoralTime.DAL.Models;

namespace CoralTime.DAL.ConvertViewToModel
{
    public static partial class ConvertViewToModel
    {
        private static void UpdatePropertiesMemberImage(MemberImage memberImage, MemberImage newMemberImage)
        {
            memberImage.MemberId = newMemberImage.MemberId;
            memberImage.FileNameImage = newMemberImage.FileNameImage;
            memberImage.ByteArrayAvatar = newMemberImage.ByteArrayAvatar;
            memberImage.ByteArrayIcon = newMemberImage.ByteArrayIcon;
        }

        public static MemberImage CreateModelForInsert(this MemberImage memberImage, MemberImage updatedMemberImage)
        {
            memberImage = new MemberImage();

            UpdatePropertiesMemberImage(memberImage, updatedMemberImage);

            return memberImage;
        }

        public static MemberImage CreateModelForUpdate(this MemberImage memberAvatar, MemberImage updatedMemberImage)
        {
            UpdatePropertiesMemberImage(memberAvatar, updatedMemberImage);

            return memberAvatar;
        }
    }
}
