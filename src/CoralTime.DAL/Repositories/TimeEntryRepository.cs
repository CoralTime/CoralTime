using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class TimeEntryRepository : GenericRepository<TimeEntry>
    {
        public TimeEntryRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<TimeEntry> GetIncludes(IQueryable<TimeEntry> query) =>
            query
                .Include(t => t.Project).ThenInclude(p => p.MemberProjectRoles).ThenInclude(mr => mr.Role)
                .Include(t => t.Member).ThenInclude(m => m.User)
                .Include(t => t.TaskType);

        // Don't touch!
        public override TimeEntry LinkedCacheGetById(int id) =>GetQuery().FirstOrDefault(x => x.Id == id);
    }
}