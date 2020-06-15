using CoralTime.DAL.Models;
using CoralTime.DAL.Models.Vsts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;


namespace CoralTime.DAL.Repositories.Vsts
{
    public class VstsProjectUserRepository : GenericRepository<VstsProjectUser>
    {
        public VstsProjectUserRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        protected override IQueryable<VstsProjectUser> GetIncludes(IQueryable<VstsProjectUser> query) =>
            query
                .Include(x => x.VstsUser)
                .Include(x => x.VstsProject);            

        public IQueryable<ApplicationUser> GetUsersByProjectId (int projectId)
        {
            return GetQuery()
                .Where(x=> x.VstsProject.ProjectId == projectId)
                .Select(x=> x.VstsUser.Member.User);
        }
    }
}