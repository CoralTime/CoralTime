using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertViewToModel;
using CoralTime.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CoralTime.DAL.Models.Member;
using System.Drawing;
using System.Drawing.Imaging;

namespace CoralTime.BL.Services
{
    public class ImageService : BaseService, IImageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IConfiguration _config;

        private static string PathAvatars() => $"{Path.Combine(Environment.CurrentDirectory, Constants.Folders.StaticFilesFolder, Constants.Folders.AvatarFolder)}";

        private static string PathIcons() => $"{Path.Combine(Environment.CurrentDirectory, Constants.Folders.StaticFilesFolder, Constants.Folders.IconFolder)}";
        

        
        public ImageService(UnitOfWork uow, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(uow, mapper)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Get Url Avatar/Icon (if can not set custom image, get url from gravatar.com) 

        public string GetUrlAvatar(int memberId) => GetUrlImageByType(memberId, Constants.ImageTypeAvatar);

        public string GetUrlIcon(int memberId) => GetUrlImageByType(memberId, Constants.ImageTypeIcon); 

        private string GetUrlImageByType(int memberId, string imageType)
        {
            var imageFileName = Uow.MemberImageRepository.LinkedCacheGetByMemberId(memberId)?.FileNameImage;

            string imagePath = null;

            if (imageFileName == null)
            {
                var memberEmail = Uow.MemberRepository.LinkedCacheGetById(memberId)?.User?.Email;
                if (memberEmail != null)
                {
                    var imageSize = GetValueImageByType(imageType, Constants.ImageTypeSizeAvatar.ToString(),
                        Constants.ImageTypeSizeIcon.ToString());
                    imagePath = $"{Constants.GravatarUrl}{GetMd5(memberEmail)}?d={Constants.GravatarType}&s={imageSize}";
                }
            }
            else
            {
                var pathFolderType = GetValueImageByType(imageType, Constants.Folders.AvatarFolder, Constants.Folders.IconFolder);
                imagePath = $"{GetStaticFileUrl()}/{pathFolderType}/{imageFileName}";
            }

            return imagePath;
        }

        private static string GetValueImageByType(string imageType, string valueImageForTypeAvatar, string valueImageForTypeIcon)
        {
            switch (imageType)
            {
                case Constants.ImageTypeAvatar:
                {
                    return valueImageForTypeAvatar;
                }
                case Constants.ImageTypeIcon:
                {
                    return valueImageForTypeIcon;
                }
                default:
                {
                    return string.Empty;
                }
            }
        }

        private static string GetMd5(string email)
        {
            byte[] data;

            using (var hasher = MD5.Create())
            {
                data = hasher.ComputeHash(Encoding.Default.GetBytes(email));
            }

            var sb = new StringBuilder();

            foreach (var d in data)
            {
                sb.Append(d.ToString("x2"));
            }

            return sb.ToString();
        }

        private string GetStaticFileUrl() => $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/{Constants.Folders.StaticFilesFolder}"; 

        #endregion

        #region Upload Image

        public string UploadImage(IFormFile uploadedFile)
        {
            var member = BaseMemberImpersonated;

            CheckFileNameAndSize(uploadedFile);

            var updatedMemberImage = CreateUpdatedMemberImage(uploadedFile, member);
            
            var memberImage = Uow.MemberImageRepository.LinkedCacheGetByMemberId(member.Id);
            if (memberImage == null)
            {
                memberImage = new MemberImage().UpdateProperties(updatedMemberImage);
                Uow.MemberImageRepository.Insert(memberImage);
            }
            else
            {
                memberImage.UpdateProperties(updatedMemberImage);
                Uow.MemberImageRepository.Update(memberImage);
            }

            Uow.Save();
            Uow.MemberImageRepository.LinkedCacheClear();

            SaveMemberImageToFolder(memberImage);

            var urlAvatar = GetUrlAvatar(member.Id);
            return urlAvatar;
        }

        private static MemberImage CreateUpdatedMemberImage(IFormFile uploadedFile, Member member)
        {
            var (byteArrayAvatar, byteArrayIcon) = CreateByteArrayFromUploadedImageFile(uploadedFile);

            var newMemberImage = new MemberImage
            {
                MemberId = member.Id,
                FileNameImage = Guid.NewGuid().ToString("N") + Path.GetExtension(uploadedFile.FileName),
                ByteArrayAvatar = byteArrayAvatar,
                ByteArrayIcon = byteArrayIcon
            };

            return newMemberImage;
        }

        private void CheckFileNameAndSize(IFormFile uploadedFile)
        {
            if (uploadedFile == null)
            {
                throw new CoralTimeEntityNotFoundException("File is empty");
            }

            var fileName = uploadedFile.FileName;
            var fileSize = uploadedFile.Length;
            
            var isFileNameValid = FileNameChecker.CheckFileName(fileName, _config["FileConstraints:PermittedExtensions"], int.Parse(_config["FileConstraints:MaxLengthFileName"]));

            var isFileSizeValid = fileSize < long.Parse(_config["FileConstraints:MaxFileSize"]);

            if (isFileNameValid != null)
            {
                throw new CoralTimeForbiddenException("FileName is Invalid");
            }

            if (!isFileSizeValid)
            {
                throw new CoralTimeForbiddenException("File size is greater than 1 Mb");
            }
        }

        private static (byte[] byteArrayAvatar, byte[] byteArrayIcon) CreateByteArrayFromUploadedImageFile(IFormFile uploadedFile)
        {
            byte[] byteArrayAvatar;
            using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
            {
                byteArrayAvatar = binaryReader.ReadBytes((int) uploadedFile.Length);
            }

            byte[] byteArrayIcon;
            using (var memoryStream = new MemoryStream())
            {
                ResizeImage(byteArrayAvatar, Constants.ImageTypeSizeIcon, Constants.ImageTypeSizeIcon).Save(memoryStream, ImageFormat.Jpeg);
                byteArrayIcon = memoryStream.ToArray();
            }

            return (byteArrayAvatar, byteArrayIcon);
        }

        private static Image ResizeImage(byte[] byteArrayOfImageFile, int horizontalSize, int verticalSize)
        {
            var imageResize = Image.FromStream(new MemoryStream(byteArrayOfImageFile));

            imageResize = imageResize.GetThumbnailImage(horizontalSize, verticalSize, () => false, IntPtr.Zero);
            return imageResize;
        }

        public void SaveImagesFromDbToFolder()
        {
            var memberImages = Uow.MemberImageRepository.GetQuery(withIncludes: false, asNoTracking : true);

            foreach (var memberImage in memberImages)
            {
                SaveMemberImageToFolder(memberImage);
            }
        }

        private static void SaveMemberImageToFolder(MemberImage memberImage)
        {
            var pathIcon = Path.Combine(PathIcons(), memberImage.FileNameImage);
            var pathAvatar = Path.Combine(PathAvatars(), memberImage.FileNameImage);

            if (!File.Exists(pathIcon) && memberImage.ByteArrayIcon != null)
            {
                File.WriteAllBytes(pathIcon, memberImage.ByteArrayIcon);
            }

            if (!File.Exists(pathAvatar) && memberImage.ByteArrayAvatar!= null)
            {
                File.WriteAllBytes(pathAvatar, memberImage.ByteArrayAvatar);
            }
        }

        #endregion
    }
}
