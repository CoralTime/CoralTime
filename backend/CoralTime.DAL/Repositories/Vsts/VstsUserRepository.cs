using CoralTime.DAL.Models;
using CoralTime.DAL.Models.Vsts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;


namespace CoralTime.DAL.Repositories.Vsts
{
    public class VstsUserRepository : GenericRepository<VstsUser>
    {
        public VstsUserRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        protected override IQueryable<VstsUser> GetIncludes(IQueryable<VstsUser> query) => query.Include(x => x.User);

        public ApplicationUser GetUserByVstsNameId(string nameId)
        {
            return GetQuery().SingleOrDefault(x => x.VstsUserId == nameId)?.User;
        }
    }
}