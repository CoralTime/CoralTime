using System.Linq;
using CoralTime.DAL.Models.Member;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories.Member
{
    public class MemberImageRepository: GenericRepository<MemberImage>
    {
        public MemberImageRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<MemberImage> GetIncludes(IQueryable<MemberImage> query) => query
            .Select(x => new MemberImage
            {
                Id = x.Id,
                MemberId = x.MemberId,
                CreationDate = x.CreationDate,
                Creator = x.Creator,
                CreatorId = x.CreatorId,
                FileNameImage = x.FileNameImage,
                LastEditor = x.LastEditor,
                LastEditorUserId = x.LastEditorUserId,
                LastUpdateDate = x.LastUpdateDate
            });

        public MemberImage LinkedCacheGetByMemberId(int memberId) =>LinkedCacheGetList().FirstOrDefault(m => m.MemberId == memberId);
    }
}
