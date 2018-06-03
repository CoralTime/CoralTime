using CoralTime.Common.Exceptions;
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

        public Member GetQueryByMemberId(int memberId) =>  GetQueryWithIncludes().FirstOrDefault(x => x.Id == memberId);

        public Member GetQueryByUserId(string userId) => GetQueryWithIncludes().FirstOrDefault(x => x.UserId == userId);

        public Member GetQueryByUserName(string userName) => GetQueryWithIncludes().FirstOrDefault(x => x.User.UserName == userName);

        #endregion

        #region LinkedCache.

        public Member LinkedCacheGetByUserNameAndCheck(string userName)
        {
            var relatedMemberByName = LinkedCacheGetByUserName(userName);
            if (relatedMemberByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with UserName: {userName} not found.");
            }

            return relatedMemberByName;
        }

        public Member LinkedCacheGetByUserName(string userName) => LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        public Member LinkedCacheGetByUserId(string userId) => LinkedCacheGetList().FirstOrDefault(m => m.User.Id == userId);

        public override Member LinkedCacheGetByName(string userName) => LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        public override Member LinkedCacheGetById(int id) => LinkedCacheGetList().FirstOrDefault(m => m.Id == id);

        #endregion
    }
}