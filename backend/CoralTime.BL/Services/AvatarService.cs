using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Interfaces;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CoralTime.BL.Services
{
    public class AvatarService : BaseService, IAvatarService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly bool _isDemo;

        public AvatarService(UnitOfWork uow, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(uow, mapper)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _isDemo = bool.Parse(_config["DemoSiteMode"]);
        }

        public void AddIconUrlInMemberView(MemberView memberView)
        {
            memberView.IconUrl = GetIconUrl(memberView.Id);
        }

        public void AddIconUrlInViewModel(IAvatarViewModel memberView)
        {
            memberView.IconUrl = GetIconUrl(memberView.MemberId);
        }

        public MemberAvatarView GetAvatar(int memberId)
        {
            return CreateAvaterMemberAvatarView(GetFileNameByMemberId(memberId), memberId);
        }

        public MemberAvatarView GetIcon(int memberId)
        {
            var member = Uow.MemberRepository.LinkedCacheGetByName(InpersonatedUserName);
            return CreateIconMemberAvatarView(GetFileNameByMemberId(memberId), memberId);
        }

        public string GetAvatarUrl(int memberId)
        {
            return GetAvatarUrl(GetFileNameByMemberId(memberId));
        }

        public string GetIconUrl(int memberId)
        {
            return GetIconUrl(GetFileNameByMemberId(memberId));
        }

        private string GetFileNameByMemberId(int memberId)
        {
            var fileName = Uow.MemberAvatarRepository.LinkedCacheGetList().FirstOrDefault(p => p.MemberId == memberId)?.AvatarFileName;
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Constants.DefaultIconFileName;
            }
            return fileName;
        }

        private bool CheckFile(string fileName, long fileSize, out string errors)
        {
            var isFileNameValid = FileNameChecker.CheckFileName(fileName, _config["FileConstraints:PermittedExtensions"], int.Parse(_config["FileConstraints:MaxLengthFileName"]), out errors);

            var isFileSizeValid = fileSize < long.Parse(_config["FileConstraints:MaxFileSize"]);

            return isFileNameValid && isFileSizeValid;
        }

        public MemberAvatarView SetUpdateMemberAvatar(IFormFile uploadedFile)
        {
            var user = Uow.UserRepository.LinkedCacheGetByName(InpersonatedUserName);
            if (user == null || !user.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"The user with userName {InpersonatedUserName} not found or is not active");
            }

            var member = Uow.MemberRepository.LinkedCacheGetByName(InpersonatedUserName);
            if (member == null)
            {
                throw new CoralTimeEntityNotFoundException($"The member for user with userName {InpersonatedUserName} not found");
            }

            if (!CheckFile(uploadedFile.FileName, uploadedFile.Length, out var errors))
            {
                throw new CoralTimeForbiddenException(errors ?? "File size is greater than 1 Mb");
            }

            byte[] imageData;

            using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)uploadedFile.Length);
            }

            var image = Image.FromStream(new MemoryStream(imageData));

            image = image.GetThumbnailImage(40, 40, () => false, IntPtr.Zero);

            byte[] thumbnaiImage;

            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Jpeg);
                thumbnaiImage = memoryStream.ToArray();
            }

            var isInsertCurrentAvatar = false;

            var memberAvatar = Uow.MemberAvatarRepository.GetQueryWithoutIncludes().FirstOrDefault(p => p.MemberId == member.Id);
            if (memberAvatar == null)
            {
                memberAvatar = new MemberAvatar();
                isInsertCurrentAvatar = true;
            }
            var newAvatarFileName = $"{(Guid.NewGuid().ToString().Replace("-", string.Empty))}{Path.GetExtension(uploadedFile.FileName)}";
            memberAvatar.MemberId = member.Id;
            memberAvatar.AvatarFile = imageData;
            memberAvatar.AvatarFileName = newAvatarFileName;
            memberAvatar.ThumbnailFile = thumbnaiImage;

            try
            {
                if (isInsertCurrentAvatar)
                {
                    Uow.MemberAvatarRepository.Insert(memberAvatar);
                }
                else
                {
                    Uow.MemberAvatarRepository.Update(memberAvatar);
                }

                Uow.Save();
                Uow.MemberAvatarRepository.LinkedCacheClear();

                SaveAvatarToFileSystem(memberAvatar);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating member avatar", e);
            }

            return CreateAvaterMemberAvatarView(memberAvatar.AvatarFileName, member.Id);
        }

        private void SaveAvatarToFileSystem(MemberAvatar avatar)
        {
            SaveAvatarToFileSystem(avatar.AvatarFileName, iconFile: avatar.ThumbnailFile, avatarFile: avatar.AvatarFile);
        }

        private void SaveAvatarToFileSystem(string fileName, byte[] iconFile, byte[] avatarFile)
        {
            var iconPath = Path.Combine(GetIconsPath(), fileName);
            var avatarPath = Path.Combine(GetAvatarsPath(), fileName);
            if (!File.Exists(iconPath))
            {
                File.WriteAllBytes(iconPath, iconFile);
            }
            if (!File.Exists(avatarPath))
            {
                File.WriteAllBytes(avatarPath, avatarFile);
            }            
        }

        private string GetAvatarsPath()
        {
            return $"{Path.Combine(Directory.GetCurrentDirectory(), Constants.Folders.StaticFilesFolder, Constants.Folders.AvatarFolder)}";
        }

        private string GetIconsPath()
        {
            return $"{Path.Combine(Directory.GetCurrentDirectory(), Constants.Folders.StaticFilesFolder, Constants.Folders.IconFolder)}";
        }

        private string GetAvatarUrl(string fileName)
        {
            return $"{GetStaticFileUrl()}/{Constants.Folders.AvatarFolder}/{fileName}";
        }

        private string GetIconUrl(string fileName)
        {
            return $"{GetStaticFileUrl()}/{Constants.Folders.IconFolder}/{fileName}";
        }

        private string GetStaticFileUrl()
        {
            return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/{Constants.Folders.StaticFilesFolder}";
        }

        private MemberAvatarView CreateIconMemberAvatarView(string fileName, int memberId)
        {
            return CreateMemberAvatarView(fileName, memberId, GetIconUrl(fileName));
        }

        private MemberAvatarView CreateAvaterMemberAvatarView(string fileName, int memberId)
        {
            return CreateMemberAvatarView(fileName, memberId, GetAvatarUrl(fileName));
        }

        private MemberAvatarView CreateMemberAvatarView(string fileName, int memberId, string url)
        {
            return new MemberAvatarView
            {
                AvatarFileName = fileName,
                MemberId = memberId,
                AvatarUrl = url
            };
        }

        public void SaveAllIconsAndAvatarsInStaticFiles()
        {
            var avatars = Uow.MemberAvatarRepository.GetQueryAsNoTraking().Where(x=> true).ToArray();

            foreach (var avatar in avatars)
            {
                SaveAvatarToFileSystem(avatar);
            }
        }
    }
}