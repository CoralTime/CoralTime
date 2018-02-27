using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class MemberAvatarRepository: BaseRepository<MemberAvatar>
    {
        public MemberAvatarRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        public override IQueryable<MemberAvatar> GetIncludes(IQueryable<MemberAvatar> query)
        {
            return query.Include(x => x.Member);
        }
    }
}
