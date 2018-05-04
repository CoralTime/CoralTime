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
    public partial class UnitOfWork
    {
        public ApplicationUser ApplicationUserCurrent { get; }

        public ApplicationUser ApplicationUserImpersonated { get; }

        public Member MemberCurrent { get; }

        public Member MemberImpersonated { get; set; }

        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly string _userId;
        public readonly string CurrentUserName;
        public readonly string ImpersonatedUserName;

        public UnitOfWork(AppDbContext appDbcontext, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _memoryCache = memoryCache;
            _context = appDbcontext;

            CurrentUserName = httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value;
            ImpersonatedUserName = GetUserNameWithImpersonation(httpContextAccessor);

            _userId = httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Properties.FirstOrDefault().Value == JwtClaimTypes.Subject)?.Value;

            ApplicationUserCurrent = GetUserCurrent();
            ApplicationUserImpersonated = GetUserImpersonated();
            MemberCurrent = GetMemberCurrent();
            MemberImpersonated = GetMemberImpersonated();
        }

        #region Get and check ApplicationUser and Member by Current and Impersonated roles;

        private ApplicationUser GetUserCurrent()
        {
            return UserRepository.LinkedCacheGetByUserNameAndCheck(CurrentUserName);
        }

        private ApplicationUser GetUserImpersonated()
        {
            return UserRepository.LinkedCacheGetByUserNameAndCheck(ImpersonatedUserName);
        }

        private Member GetMemberCurrent()
        {
            var userCurrent = GetUserCurrent();
            var memberCurrent = MemberRepository.LinkedCacheGetByUserId(userCurrent?.Id);

            return memberCurrent;
        }

        private Member GetMemberImpersonated()
        {
            var userImpersonated = GetUserImpersonated();
            var memberImpersonated = MemberRepository.LinkedCacheGetByUserId(userImpersonated?.Id);

            return memberImpersonated;
        }

        #endregion Get and check ApplicationUser and Member by Current and Impersonated roles;

        public int Save()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new CoralTimeUpdateException("You try to save not changes data.", e);
            }
            catch (DbUpdateException e)
            {
                throw new CoralTimeInsertDublicateException("Insert dublicate data for unique field", e);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("Other error related with Save data.", e);
            }
        }

        private string GetUserNameWithImpersonation(IHttpContextAccessor httpContextAccessor)
        {
            if (!HasAuthorizationHeader(httpContextAccessor))
                return string.Empty;

            var currentUserClaims = httpContextAccessor?.HttpContext?.User?.Claims;

            if (HasImpersonationHeader(httpContextAccessor, out var headerImpersonatedUserName))
            {
                if (currentUserClaims.FirstOrDefault(c => c.Properties.FirstOrDefault().Value == JwtClaimTypes.Role)?.Value != Constants.ApplicationRoleAdmin)
                {
                    throw new CoralTimeForbiddenException($"You can not impersonate user with userName {headerImpersonatedUserName}");
                }

                return headerImpersonatedUserName;
            }

            var headerAuthorizationUserName = currentUserClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name).Value;
            return headerAuthorizationUserName;
        }

        private static bool HasAuthorizationHeader(IHttpContextAccessor httpContextAccessor)
        {
            return GetHeaderValue(httpContextAccessor, Constants.HeaderNameAuthorization, out var headerAuthorizationValue);
        }

        private static bool HasImpersonationHeader(IHttpContextAccessor httpContextAccessor, out StringValues headerImpersonatedUserValue)
        {
            return GetHeaderValue(httpContextAccessor, Constants.ImpersonatedUserNameHeader, out headerImpersonatedUserValue);
        }

        private static bool GetHeaderValue(IHttpContextAccessor httpContextAccessor, string headerName, out StringValues headerValue)
        {
            return httpContextAccessor?.HttpContext?.Request?.Headers?.TryGetValue(headerName, out headerValue) ?? false;
        }
    }
}