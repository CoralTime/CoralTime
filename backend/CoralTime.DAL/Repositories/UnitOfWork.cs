using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class UnitOfWork
    {
        #region List of Repositories 

        private ProjectRepository _projectRepository;
        public ProjectRepository ProjectRepository => _projectRepository ?? (_projectRepository = new ProjectRepository(AppDbContext, MemoryCache, UserId));

        private ProjectRoleRepository _projectRoleRepository;
        public ProjectRoleRepository ProjectRoleRepository => _projectRoleRepository ?? (_projectRoleRepository = new ProjectRoleRepository(AppDbContext, MemoryCache, UserId));

        private TimeEntryRepository _timeEntryRepository;
        public TimeEntryRepository TimeEntryRepository => _timeEntryRepository ?? (_timeEntryRepository = new TimeEntryRepository(AppDbContext, MemoryCache, UserId));

        private ClientRepository _clientRepository;
        public ClientRepository ClientRepository => _clientRepository ?? (_clientRepository = new ClientRepository(AppDbContext, MemoryCache, UserId));

        private TaskTypeRepository _taskTypeRepository;
        public TaskTypeRepository TaskTypeRepository => _taskTypeRepository ?? (_taskTypeRepository = new TaskTypeRepository(AppDbContext, MemoryCache, UserId));

        private MemberRepository _memberRepository;
        public MemberRepository MemberRepository => _memberRepository ?? (_memberRepository = new MemberRepository(AppDbContext, MemoryCache, UserId));

        private UserRepository _userRepository;
        public UserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(AppDbContext, MemoryCache, UserId));

        private MemberProjectRoleRepository _memberProjectRoleRepository;
        public MemberProjectRoleRepository MemberProjectRoleRepository => _memberProjectRoleRepository ?? (_memberProjectRoleRepository = new MemberProjectRoleRepository(AppDbContext, MemoryCache, UserId));

        private UserForgotPassRequestRepository _userForgotPassRequestRepository;
        public UserForgotPassRequestRepository UserForgotPassRequestRepository => _userForgotPassRequestRepository ?? (_userForgotPassRequestRepository = new UserForgotPassRequestRepository(AppDbContext, MemoryCache, UserId));

        private MemberImageRepository _memberImageRepository;
        public MemberImageRepository MemberImageRepository => _memberImageRepository ?? (_memberImageRepository = new MemberImageRepository(AppDbContext, MemoryCache, UserId));

        private ReportsSettingsRepository _reportsSettingsRepository;
        public ReportsSettingsRepository ReportsSettingsRepository => _reportsSettingsRepository ?? (_reportsSettingsRepository = new ReportsSettingsRepository(AppDbContext, MemoryCache, UserId));

        #endregion

        private AppDbContext AppDbContext { get; }
        private IMemoryCache MemoryCache { get; }
        private string UserId { get; }

        public Member MemberCurrent { get; }
        public Member MemberImpersonated { get; }

        public UnitOfWork(AppDbContext appDbContext, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            AppDbContext = appDbContext;
            MemoryCache = memoryCache;
            UserId = GetUserIdFromHttpContext(httpContextAccessor);

            var userNameCurrent = GetUserNameCurrentFromHttpContext(httpContextAccessor);
            var userNameImpersonated = GetUserNameImpersonatedFromHttpContext(httpContextAccessor);

            CheckApplicationUsers(userNameCurrent, userNameImpersonated);

            MemberCurrent = MemberRepository.LinkedCacheGetByUserNameAndCheck(userNameCurrent);
            MemberImpersonated = MemberRepository.LinkedCacheGetByUserNameAndCheck(userNameImpersonated);
        }

        private void CheckApplicationUsers(string userNameCurrent, string userNameImpersonated)
        {
            UserRepository.LinkedCacheGetByUserNameAndCheck(userNameCurrent);
            UserRepository.LinkedCacheGetByUserNameAndCheck(userNameImpersonated);
        }
        
        #region Get User Names from Http Context from Headers

        private string GetUserIdFromHttpContext(IHttpContextAccessor httpContextAccessor) =>
            httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Properties.FirstOrDefault().Value == JwtClaimTypes.Subject)?.Value;

        private string GetUserNameCurrentFromHttpContext(IHttpContextAccessor httpContextAccessor) => 
            httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value;

        #region Get Impersonated UserName From HttpContext

        private string GetUserNameImpersonatedFromHttpContext(IHttpContextAccessor httpContextAccessor)
        {
            if (!HasAuthorizationHeader(httpContextAccessor))
            {
                return string.Empty;
            }

            var currentUserClaims = httpContextAccessor?.HttpContext?.User?.Claims;

            if (HasImpersonationHeader(httpContextAccessor, out var headerImpersonatedUserName))
            {
                if (currentUserClaims.FirstOrDefault(c => c.Properties.FirstOrDefault().Value == JwtClaimTypes.Role)?.Value != Constants.ApplicationRoleAdmin)
                {
                    throw new CoralTimeForbiddenException("Impersonate mode is available only for Admin role");
                }

                return headerImpersonatedUserName;
            }

            var userNameCurrentFromHeader = currentUserClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name).Value;
            return userNameCurrentFromHeader;
        }

        private bool HasAuthorizationHeader(IHttpContextAccessor httpContextAccessor) => 
            GetHeaderValue(httpContextAccessor, Constants.HeaderNameAuthorization, out var headerAuthorizationValue);

        private bool HasImpersonationHeader(IHttpContextAccessor httpContextAccessor, out StringValues headerImpersonatedUserValue) => 
            GetHeaderValue(httpContextAccessor, Constants.ImpersonatedUserNameHeader, out headerImpersonatedUserValue);

        private bool GetHeaderValue(IHttpContextAccessor httpContextAccessor, string headerName, out StringValues headerValue) => 
            httpContextAccessor?.HttpContext?.Request?.Headers?.TryGetValue(headerName, out headerValue) ?? false;

        #endregion

        #endregion

        public int Save()
        {
            try
            {
                return AppDbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new CoralTimeUpdateException("You are trying to save data that has not been changed", e);
            }
            catch (DbUpdateException e)
            {
                throw new CoralTimeInsertDublicateException("You are trying to insert data that already exists for a unique field", e);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("Other errors related to data saving", e);
            }
        }
    }
}