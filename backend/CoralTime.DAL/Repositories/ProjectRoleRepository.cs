using System.Linq;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories
{
    public class ProjectRoleRepository : GenericRepository<ProjectRole>
    {
        public ProjectRoleRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<ProjectRole> GetIncludes(IQueryable<ProjectRole> query) => query;

        public int GetManagerRoleId() => LinkedCacheGetList().FirstOrDefault(z => z.Name == Constants.ProjectRoleManager).Id;

        public int GetMemberRoleId() => LinkedCacheGetList().FirstOrDefault(z => z.Name == Constants.ProjectRoleMember).Id;

        public ProjectRole GetMemberRole() =>  LinkedCacheGetList().FirstOrDefault(z => z.Name == Constants.ProjectRoleMember);
    }
}