using CoralTime.DAL.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using CoralTime.Common.Exceptions;
using IdentityServer4.Extensions;

namespace CoralTime.DAL.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>
    {
        public UserRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        public override ApplicationUser LinkedCacheGetByName(string userName)
        {
            return LinkedCacheGetList().FirstOrDefault(p => p.UserName == userName);
        }

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