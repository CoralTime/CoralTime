using System.Collections.Generic;
using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class MemberImageRepository: BaseRepository<MemberImage>
    {
        public MemberImageRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        public override IQueryable<MemberImage> GetIncludes(IQueryable<MemberImage> query)
        {
            return query.Include(x => x.Member).Select(x => new MemberImage
            {
                Id = x.Id,
                Member = x.Member,
                MemberId = x.MemberId,
                CreationDate = x.CreationDate,
                Creator = x.Creator,
                CreatorId = x.CreatorId,
                FileNameImage = x.FileNameImage,
                LastEditor = x.LastEditor,
                LastEditorUserId = x.LastEditorUserId,
                LastUpdateDate = x.LastUpdateDate
            });
        }

        public MemberImage LinkedCacheGetByMemberId(int memberId)
        {
            return LinkedCacheGetList().FirstOrDefault(m => m.MemberId == memberId);
        }
    }
}
