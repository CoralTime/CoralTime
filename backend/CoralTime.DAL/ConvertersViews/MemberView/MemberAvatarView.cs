using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Profiles;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static MemberAvatarView GetViewMemberIcon(this MemberAvatar memberAvatar, IMapper _mapper, int memberId)
        {
            var memberView = new MemberAvatarView();

            if (memberAvatar != null)
            {
                memberView = _mapper.Map<MemberAvatar, MemberAvatarView>(memberAvatar);
                memberView.AvatarFile = memberAvatar.ThumbnailFile;
            }
            else
            {
                memberView = GetViewEmptyMemberAvatar(memberId);
            }

            return memberView;
        }

        public static MemberAvatarView GetViewMemberAvatar(this MemberAvatar memberAvatar, IMapper _mapper, int memberId)
        {
            var memberView = new MemberAvatarView();
            
            if (memberAvatar != null)
            {
                memberView = _mapper.Map<MemberAvatar, MemberAvatarView>(memberAvatar);
                memberView.AvatarFile = memberAvatar.AvatarFile;
            }
            else
            {
                memberView = GetViewEmptyMemberAvatar(memberId);
            }

            return memberView;
        }

        private static MemberAvatarView GetViewEmptyMemberAvatar(int memberId)
        {
            return new MemberAvatarView
            {
                AvatarFile = new byte[0],
                AvatarFileName = string.Empty,
                MemberId = memberId
            };
        }
    }
}