using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;

namespace CoralTime.BL.Interfaces
{
    public interface IImageService
    {
        string GetUrlAvatar(int memberId);

        string GetUrlIcon(int memberId);

        MemberImageView UploadImage(IFormFile uploadedFile);

        void SaveImagesFromDbToFolder();
    }
}