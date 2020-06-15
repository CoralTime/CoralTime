using CoralTime.DAL.Models.Vsts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories.Vsts
{
    public class VstsProjectRepository : GenericRepository<VstsProject>
    {
        public VstsProjectRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        protected override IQueryable<VstsProject> GetIncludes(IQueryable<VstsProject> query) => query
            .Include(x => x.Project)
            .Include(x=> x.VstsProjectUsers);
    }
}