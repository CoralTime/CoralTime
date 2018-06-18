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

        public override IQueryable<MemberProjectRole> GetIncludes(IQueryable<MemberProjectRole> query) => query
            .Include(p => p.Role)
            .Include(p => p.Member)
            .Include(p => p.Member).ThenInclude(m => m.User)
            .Include(r => r.Member).ThenInclude(m => m.TimeEntries)
            .Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Project)
            .Include(p => p.Project).ThenInclude(p => p.Client)
            .Include(p => p.Project).ThenInclude(p => p.TaskTypes)
            .Include(p => p.Project).ThenInclude(p => p.MemberProjectRoles).ThenInclude(p => p.Project);
            //// TODO del? 
            //.Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Member)
            //.Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Role)
            //.Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Member)
            //.ThenInclude(x => x.User)
            //.Include(p => p.Project);

        public override MemberProjectRole LinkedCacheGetById(int Id) => LinkedCacheGetList().FirstOrDefault(p => p.Id == Id);
    }
}