using System.Linq;
using CoralTime.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MemberModel = CoralTime.DAL.Models.Member.Member;

namespace CoralTime.DAL.Repositories.Member
{
    public class MemberRepository : GenericRepository<MemberModel>
    {
        public MemberRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        #region Get Query.

        protected override IQueryable<MemberModel> GetIncludes(IQueryable<MemberModel> query)
        {
            return query
                    .Include(m => m.User)
                    .Include(m => m.MemberProjectRoles).ThenInclude(m => m.Project); 
        }

        public MemberModel GetQueryByMemberId(int memberId) =>  GetQuery().FirstOrDefault(x => x.Id == memberId);

        public MemberModel GetQueryByUserName(string userName) => GetQuery().FirstOrDefault(x => x.User.UserName == userName);

        #endregion

        #region LinkedCache.

        public MemberModel LinkedCacheGetByUserNameAndCheck(string userName)
        {
            var relatedMemberByName = LinkedCacheGetByUserName(userName);
            if (relatedMemberByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with UserName: {userName} not found.");
            }

            return relatedMemberByName;
        }

        public MemberModel LinkedCacheGetByUserName(string userName) => LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        public MemberModel LinkedCacheGetByUserId(string userId) => LinkedCacheGetList().FirstOrDefault(m => m.User.Id == userId);

        public override MemberModel LinkedCacheGetByName(string userName) => LinkedCacheGetList().FirstOrDefault(m => m.User.UserName == userName);
        public override MemberModel LinkedCacheGetById(int id) => LinkedCacheGetList().FirstOrDefault(m => m.Id == id);

        #endregion
    }
}