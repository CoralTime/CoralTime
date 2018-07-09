using System.Linq;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.Models;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories.User
{
    public class UserRepository : GenericRepository<ApplicationUser>
    {
        public UserRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<ApplicationUser> GetIncludes(IQueryable<ApplicationUser> query) => query;

        public override ApplicationUser LinkedCacheGetByName(string userName) => LinkedCacheGetList().FirstOrDefault(p => p.UserName == userName);

        public ApplicationUser LinkedCacheGetByUserNameAndCheck(string userName)
        {
            if (userName.IsNullOrEmpty())
            {
                return null;
            }
            
            var relatedUserByName = LinkedCacheGetByName(userName);
            if (relatedUserByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"User {userName} not found.");
            }

            if (!relatedUserByName.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"User {userName} is not active.");
            }

            return relatedUserByName;
        }
    }
}