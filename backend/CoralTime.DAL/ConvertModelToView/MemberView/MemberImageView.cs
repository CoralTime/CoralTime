using CoralTime.DAL.Models;
using CoralTime.ViewModels.Profiles;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static MemberImageView GetView(this MemberImage memberImager, string urlAvatar)
        {
            return new MemberImageView
            {
                MemberId = memberImager.MemberId,
                FileNameAvatar = memberImager.FileNameImage,
                UrlAvatar = urlAvatar,
                ByteArrayAvatar = memberImager.ByteArrayAvatar
            };
        }
    }
}