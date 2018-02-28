using CoralTime.DAL.Models;
using CoralTime.ViewModels;

namespace CoralTime.DAL.ConvertersOfModels
{
    public static partial class ConvertersOfModels
    {
        private static void MapPropertyForModel(MemberAvatar memberAvatar, MemberAvatarPropertiesView memberAvatarPropertiesView)
        {
            memberAvatar.MemberId = memberAvatarPropertiesView.MemberId;
            memberAvatar.AvatarFile = memberAvatarPropertiesView.AvatarFile;
            memberAvatar.AvatarFileName = memberAvatarPropertiesView.AvatarFileName;
            memberAvatar.ThumbnailFile = memberAvatarPropertiesView.ThumbnailFile;
        }

        public static MemberAvatar CreateModelForInsert(this MemberAvatar memberAvatar, MemberAvatarPropertiesView memberAvatarPropertiesView)
        {
            memberAvatar = new MemberAvatar();

            MapPropertyForModel(memberAvatar, memberAvatarPropertiesView);

            return memberAvatar;
        }

        public static MemberAvatar CreateModelForUpdate(this MemberAvatar memberAvatar, MemberAvatarPropertiesView memberAvatarPropertiesView)
        {
            MapPropertyForModel(memberAvatar, memberAvatarPropertiesView);

            return memberAvatar;
        }
    }
}
