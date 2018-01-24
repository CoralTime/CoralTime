using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class ProjectRepository : BaseRepository<Project>
    {
        public ProjectRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        public override IQueryable<Project> GetIncludes(IQueryable<Project> query)
        {
            return query
                .Include(p => p.Client)
                .Include(p => p.MemberProjectRoles).ThenInclude(x => x.Member).ThenInclude(x => x.User)
                .Include(p => p.MemberProjectRoles).ThenInclude(x => x.Role)
                .Include(p => p.TaskTypes)
                .Include(m => m.Creator);
        }

        #region LinkedCache.

        public override Project LinkedCacheGetByName(string name)
        {
            return LinkedCacheGetList().FirstOrDefault(p => p.Name == name);
        }

        public override Project LinkedCacheGetById(int projectId)
        {
            return LinkedCacheGetList().FirstOrDefault(p => p.Id == projectId);
        }

        #endregion
    }
}