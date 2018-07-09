using System.Linq;
using CoralTime.DAL.Models.Member;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories.Member
{
    public class MemberProjectRoleRepository : GenericRepository<MemberProjectRole>
    {
        public MemberProjectRoleRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<MemberProjectRole> GetIncludes(IQueryable<MemberProjectRole> query) => query
            .Include(p => p.Role)
            .Include(p => p.Member)
            .Include(p => p.Member).ThenInclude(m => m.User)
            .Include(p => p.Project).ThenInclude(p => p.Client)
            .Include(p => p.Project).ThenInclude(p => p.TaskTypes);

        public override MemberProjectRole LinkedCacheGetById(int id) => LinkedCacheGetList().FirstOrDefault(p => p.Id == id);
    }
}