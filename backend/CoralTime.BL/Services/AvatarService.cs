using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CoralTime.DAL.ConvertersOfModels;
using CoralTime.ViewModels;

namespace CoralTime.BL.Services
{
    public class AvatarService : BaseService, IAvatarService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public AvatarService(UnitOfWork uow, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(uow, mapper)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUrlAvatar(int memberId)
        {
            return GetUrlImage(memberId, Constants.ImageTypeAvatar);
        }

        public string GetUrlIcon(int memberId)
        {
            return GetUrlImage(memberId, Constants.ImageTypeIcon);
        }

        private string GetUrlImage(int memberId, string imageType)
        {
            var imageFileName = GetImageFileNameByMemberId(memberId);

            string imagePath = null;

            // Try to get Gravatar url if user don't have custom Image
            if (imageFileName == Constants.DefaultIconFileName)
            {
                var memberEmail = Uow.MemberRepository.LinkedCacheGetById(memberId).User?.Email;
                if (memberEmail != null)
                {
                    imagePath = GetUrlImageByGravatarService(memberEmail, imageType);
                }
            }
            else
            {
                if (imageType == Constants.ImageTypeAvatar)
                {
                    imagePath = $"{GetStaticFileUrl()}/{Constants.Folders.AvatarFolder}/{imageFileName}";
                }

                if (imageType == Constants.ImageTypeIcon)
                {
                    imagePath = $"{GetStaticFileUrl()}/{Constants.Folders.IconFolder}/{imageFileName}";
                }
            }

            return imagePath;
        }

        private string GetImageFileNameByMemberId(int memberId)
        {
            var imageFileName = Uow.MemberAvatarRepository.LinkedCacheGetByMemberId(memberId)?.AvatarFileName;

            imageFileName = string.IsNullOrEmpty(imageFileName)
                ? Constants.DefaultIconFileName
                : imageFileName;

            return imageFileName;
        }

        private string GetUrlImageByGravatarService(string memberEmail, string imageType)
        {
            var imageSize = string.Empty;

            if (imageType == Constants.ImageTypeAvatar)
            {
                imageSize = "200";
            }

            if (imageType == Constants.ImageTypeIcon)
            {
                imageSize = "40";
            }

            return $"http://www.gravatar.com/avatar/{GetMD5(memberEmail)}?d={"wavatar"}&s={imageSize}";
        }

        public MemberAvatarView SetUpdateMemberAvatar(IFormFile uploadedFile)
        {
            var user = Uow.UserRepository.GetRelatedUserByName(Uow.InpersonatedUserName);
            var member = Uow.MemberRepository.GetQueryByUserId(user.Id);

            if (!CheckFile(uploadedFile.FileName, uploadedFile.Length, out var errors))
            {
                throw new CoralTimeForbiddenException(errors ?? "File size is greater than 1 Mb");
            }

            byte[] byteArrayImage = CreateByteArrayFromUploadedImageFile(uploadedFile);
            byte[] byteArrayImageThumbnail = CreateByteArrayFromImageThumbnail(byteArrayImage);

            var memberAvatarPropertiesView = new MemberAvatarPropertiesView
            {
                MemberId = member.Id,
                AvatarFile = byteArrayImage,
                AvatarFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(uploadedFile.FileName),
                ThumbnailFile = byteArrayImageThumbnail
            };

            var memberAvatar = Uow.MemberAvatarRepository.LinkedCacheGetByMemberId(member.Id);
            if (memberAvatar == null)
            {
                memberAvatar.CreateModelForInsert(memberAvatarPropertiesView);

                Uow.MemberAvatarRepository.Insert(memberAvatar);
            }
            else
            {
                memberAvatar.CreateModelForUpdate(memberAvatarPropertiesView);

                Uow.MemberAvatarRepository.Update(memberAvatar);
            }

            Uow.Save();
            Uow.MemberAvatarRepository.LinkedCacheClear();

            SaveMemberAvatarToFileSystem(memberAvatar);

            return CreateAvatarMemberAvatarView(memberAvatar.AvatarFileName, member.Id);
        }

        private bool CheckFile(string fileName, long fileSize, out string errors)
        {
            var isFileNameValid = FileNameChecker.CheckFileName(fileName, _config["FileConstraints:PermittedExtensions"], int.Parse(_config["FileConstraints:MaxLengthFileName"]), out errors);

            var isFileSizeValid = fileSize < long.Parse(_config["FileConstraints:MaxFileSize"]);

            return isFileNameValid && isFileSizeValid;
        }

        private static byte[] CreateByteArrayFromImageThumbnail(byte[] arrayOfBytesFromImage)
        {
            byte[] arrayOfBytesFromImageThumbnail;

            using (var memoryStream = new MemoryStream())
            {
                ResizeImage(arrayOfBytesFromImage).Save(memoryStream, ImageFormat.Jpeg);
                arrayOfBytesFromImageThumbnail = memoryStream.ToArray();
            }

            return arrayOfBytesFromImageThumbnail;
        }

        private static byte[] CreateByteArrayFromUploadedImageFile(IFormFile uploadedFile)
        {
            byte[] byteArrayOfImageFile;
            using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
            {
                byteArrayOfImageFile = binaryReader.ReadBytes((int) uploadedFile.Length);
            }

            return byteArrayOfImageFile;
        }

        private static Image ResizeImage(byte[] byteArrayOfImageFile)
        {
            var imageResize = Image.FromStream(new MemoryStream(byteArrayOfImageFile));

            imageResize = imageResize.GetThumbnailImage(40, 40, () => false, IntPtr.Zero);
            return imageResize;
        }

        private string GetAvatarsPath()
        {
            return $"{Path.Combine(Directory.GetCurrentDirectory(), Constants.Folders.StaticFilesFolder, Constants.Folders.AvatarFolder)}";
        }

        private string GetIconsPath()
        {
            return $"{Path.Combine(Directory.GetCurrentDirectory(), Constants.Folders.StaticFilesFolder, Constants.Folders.IconFolder)}";
        }

        private string GetStaticFileUrl()
        {
            return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/{Constants.Folders.StaticFilesFolder}";
        }

        private MemberAvatarView CreateIconMemberAvatarView(string fileName, int memberId)
        {
            return CreateMemberAvatarView(fileName, memberId, GetUrlIcon(memberId));
        }

        private MemberAvatarView CreateAvatarMemberAvatarView(string fileName, int memberId)
        {
            return CreateMemberAvatarView(fileName, memberId, GetUrlAvatar(memberId));
        }

        private MemberAvatarView CreateMemberAvatarView(string fileName, int memberId, string url)
        {
            var memberAvatarView = new MemberAvatarView
            {
                AvatarFileName = fileName,
                MemberId = memberId,
                AvatarUrl = url
            };

            return memberAvatarView;
        }

        public void SaveIconsAndAvatarsToStaticFiles()
        {
            var memberAvatars = Uow.MemberAvatarRepository.GetQueryAsNoTraking().Where(x => true).ToArray();

            foreach (var memberAvatar in memberAvatars)
            {
                SaveMemberAvatarToFileSystem(memberAvatar);
            }
        }

        private void SaveMemberAvatarToFileSystem(MemberAvatar avatar)
        {
            var iconPath = Path.Combine(GetIconsPath(), avatar.AvatarFileName);
            var avatarPath = Path.Combine(GetAvatarsPath(), avatar.AvatarFileName);

            if (!File.Exists(iconPath))
            {
                File.WriteAllBytes(iconPath, avatar.ThumbnailFile);
            }

            if (!File.Exists(avatarPath))
            {
                File.WriteAllBytes(avatarPath, avatar.AvatarFile);
            }
        }

        private static string GetMD5(string email)
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
    }
}
