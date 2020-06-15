using System.Linq;
using CoralTime.DAL.Models.LogChanges;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories
{
    public class MemberActionRepository: GenericRepository<MemberAction>
    {
        public MemberActionRepository (AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<MemberAction> GetIncludes(IQueryable<MemberAction> query) => query.Include(x => x.Member);
    }
}