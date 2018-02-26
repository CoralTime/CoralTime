using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    // TODO Rewrite  Convert -> Convert To with mapper!
    public class MemberProjectRoleRepository : BaseRepository<MemberProjectRole>
    {
        private readonly AppDbContext _context;

        public MemberProjectRoleRepository(AppDbContext context, IMemoryCache memoryCache, string userId) : base(context, memoryCache, userId)
        {
            _context = context;
        }

        public int GetManagerRoleId()
        {
            return _context.ProjectRoles.FirstOrDefault(r => r.Name.Equals(Constants.ProjectRoleManager)).Id;
        }

        public override MemberProjectRole LinkedCacheGetById(int Id)
        {
            return LinkedCacheGetList().FirstOrDefault(p => p.Id == Id);
        }
        
        public override IQueryable<MemberProjectRole> GetIncludes(IQueryable<MemberProjectRole> query)
        {
            return query
                .Include(p => p.Role)
                .Include(p => p.Member)
                .Include(p => p.Member).ThenInclude(m => m.User)
                .Include(r => r.Member).ThenInclude(m => m.TimeEntries)
                .Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Project)
                .Include(p => p.Project).ThenInclude(p => p.Client)
                .Include(p => p.Project).ThenInclude(p => p.TaskTypes)
                .Include(p => p.Project).ThenInclude(p => p.MemberProjectRoles).ThenInclude(p => p.Project)

                // TODO del? 
                .Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Member)
                .Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Role)
                .Include(r => r.Member).ThenInclude(m => m.MemberProjectRoles).ThenInclude(m => m.Member)
                .ThenInclude(x => x.User)
                .Include(p => p.Project);
        }
    }
}