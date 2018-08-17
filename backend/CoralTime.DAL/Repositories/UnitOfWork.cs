using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoralTime.DAL.Models.LogChanges;
using CoralTime.DAL.Repositories.Member;
using CoralTime.DAL.Repositories.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MemberActionTypes = CoralTime.Common.Constants.Constants.MemberActionTypes;

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
        
        private MemberActionRepository _memberActionRepository;
        public MemberActionRepository MemberActionRepository => _memberActionRepository ?? (_memberActionRepository = new MemberActionRepository(AppDbContext, MemoryCache, UserId));

        #endregion

        private AppDbContext AppDbContext { get; }
        private IMemoryCache MemoryCache { get; }
        private string UserId { get; }

        public Models.Member.Member MemberCurrent { get; }
        public Models.Member.Member MemberImpersonated { get; }

        public UnitOfWork(AppDbContext appDbContext, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            AppDbContext = appDbContext;
            MemoryCache = memoryCache;
            UserId = GetUserIdFromHttpContext(httpContextAccessor);

            var userNameCurrent = GetUserNameCurrentFromHttpContext(httpContextAccessor);
            var userNameImpersonated = GetUserNameImpersonatedFromHttpContext(httpContextAccessor);

            if (userNameCurrent != null && userNameImpersonated != null)
            {
                CheckApplicationUsers(userNameCurrent, userNameImpersonated);

                MemberCurrent = MemberRepository.LinkedCacheGetByUserNameAndCheck(userNameCurrent);
                MemberImpersonated = MemberRepository.LinkedCacheGetByUserNameAndCheck(userNameImpersonated);
            }
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

        #endregion
        
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

        public int Save()
        {
            try
            {
                var actions = CreateMemberActions();
                var changesCount = AppDbContext.SaveChanges();
                SaveMemberActions(actions);
                return changesCount;
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

        public void SingleCacheClearByKey(string key) => MemoryCache.Remove(key);

        #region Save MemberActions

        private IEnumerable<MemberAction> CreateMemberActions()
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss"
            };
            var modifiedEntries = AppDbContext.ChangeTracker
                .Entries()
                .Where(x => x.State != EntityState.Unchanged)
                .Select(x => new {x.Entity, x.State, x.CurrentValues, x.OriginalValues})
                .ToList();
            
            var actions = new List<MemberAction>();
            
            foreach (var entry in modifiedEntries)
            {
                var pkName = $"<{(entry.CurrentValues.Properties.SingleOrDefault(x => x.IsPrimaryKey())?.Name)}>";
                var oldValue = entry.OriginalValues.ToObject();
                var newValue = entry.CurrentValues.ToObject();
                var actionType = GetActionType(entry.State);

                var action = new MemberAction
                {
                    MemberId = MemberCurrent.Id,
                    ChangedObject = JsonConvert.SerializeObject(newValue, jsonSettings),
                    Entity = entry.Entity.GetType().Name,
                    ChangedFields = JsonConvert.SerializeObject(DetailedCompare(newValue: newValue, oldValue: oldValue), jsonSettings),
                    EntityId = GetId(newValue, pkName, actionType),
                    Action = actionType.ToString()
                };

                actions.Add(action);
            }
            return actions;
        }

        private void SaveMemberActions(IEnumerable<MemberAction> actions)
        {
            AppDbContext.MemberActions.AddRange(actions);
            AppDbContext.SaveChanges();
        }

        private static string GetId<T>(T item, string pkName, MemberActionTypes actionType)
        {
            if (actionType == MemberActionTypes.Add)
                return null;
            
            var fields = item.GetType()
                .GetFields(BindingFlags.Instance |
                           BindingFlags.Static |
                           BindingFlags.NonPublic |
                           BindingFlags.Public);
            var field = fields.SingleOrDefault(x => x.Name.StartsWith(pkName));
            var val = field?.GetValue(item);
            return val?.ToString();
        }
        
        private static List<Variance> DetailedCompare<T>(T newValue, T oldValue)
        {
            var propertyInfo = newValue.GetType().GetProperties();
            return propertyInfo.Select(f => new Variance
                {
                    Field = f.Name,
                    NewValue = f.GetValue(newValue),
                    OldValue = f.GetValue(oldValue)
                })
                .Where(v => (v.OldValue != null  && !v.OldValue.Equals(v.NewValue))|| (v.NewValue != null  && !v.NewValue.Equals(v.OldValue)))
                .ToList();
        }

        private static MemberActionTypes GetActionType(EntityState state)
        {
            switch (state)
            {
                case EntityState.Deleted:
                    return MemberActionTypes.Delete;
                case EntityState.Modified:
                    return MemberActionTypes.Change;
                case EntityState.Added:
                    return MemberActionTypes.Add;
                default:
                    return MemberActionTypes.None;
            }
        }

        private class Variance
        {
            public string Field { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }

        #endregion Save MemberActions
    }
}