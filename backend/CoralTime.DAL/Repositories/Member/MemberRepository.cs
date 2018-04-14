using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class MemberRepository : BaseRepository<Member>
    {
        public MemberRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        #region Get Query.

        public override IQueryable<Member> GetIncludes(IQueryable<Member> query)
        {
            return query
                .Include(m => m.User)
                .Include(m => m.MemberProjectRoles).ThenInclude(m => m.Project)
                .Include(m => m.Creator)
                .Include(m => m.TimeEntries);
        }

        public Member GetQueryByMemberId(int memberId)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.Id == memberId);
        }

        public Member GetQueryByUserId(string userId)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.UserId == userId);
        }

        public Member GetQueryByUserName(string userName)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.User.UserName == userName);
        }

        #endregion

        #region LinkedCache.

        public override Member LinkedCacheGetByName(string userName)
        {
            return LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        }

        public Member LinkedCacheGetByUserId(string userId)
        {
            return LinkedCacheGetList().FirstOrDefault(m => m.User.Id == userId);
        }

        public Member LinkedCacheGetByUserName(string userName)
        {
            return LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        }

        public override Member LinkedCacheGetById(int id)
        {
            return LinkedCacheGetList().FirstOrDefault(m => m.Id == id);
        }

        #endregion
    }
}