using System.Linq;
using CoralTime.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories.Member
{
    public class MemberRepository : GenericRepository<Models.Member.Member>
    {
        public MemberRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        #region Get Query.

        public override IQueryable<Models.Member.Member> GetIncludes(IQueryable<Models.Member.Member> query)
        {
            return query
                .Include(m => m.User)
                .Include(m => m.MemberProjectRoles).ThenInclude(m => m.Project)
                .Include(m => m.Creator)
                .Include(m => m.TimeEntries);
        }

        public Models.Member.Member GetQueryByMemberId(int memberId) =>  GetQuery().FirstOrDefault(x => x.Id == memberId);

        public Models.Member.Member GetQueryByUserId(string userId) => GetQuery().FirstOrDefault(x => x.UserId == userId);

        public Models.Member.Member GetQueryByUserName(string userName) => GetQuery().FirstOrDefault(x => x.User.UserName == userName);

        #endregion

        #region LinkedCache.

        public Models.Member.Member LinkedCacheGetByUserNameAndCheck(string userName)
        {
            var relatedMemberByName = LinkedCacheGetByUserName(userName);
            if (relatedMemberByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with UserName: {userName} not found.");
            }

            return relatedMemberByName;
        }

        public Models.Member.Member LinkedCacheGetByUserName(string userName) => LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        public Models.Member.Member LinkedCacheGetByUserId(string userId) => LinkedCacheGetList().FirstOrDefault(m => m.User.Id == userId);

        public override Models.Member.Member LinkedCacheGetByName(string userName) => LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        public override Models.Member.Member LinkedCacheGetById(int id) => LinkedCacheGetList().FirstOrDefault(m => m.Id == id);

        #endregion
    }
}