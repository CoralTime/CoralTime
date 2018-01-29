using System.Collections.Generic;
using CoralTime.DAL.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class ReportsSettingsRepository : BaseRepository<ReportsSettings>
    {
        public ReportsSettingsRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        public List<ReportsSettings> GetQueriesByMemberIdWithIncludes(int memberId)
        {
            return GetQueryWithIncludes().Where(x => x.MemberId == memberId).ToList();
        }

        public IQueryable<ReportsSettings> GetEntitiesOutOfContext()
        {
            return GetQueryAsNoTrakingWithIncludes();
        }

        public ReportsSettings GetEntitiesOutOfContextForThisMemberById(int id, int memberId)
        {
            return GetQueryAsNoTrakingWithIncludes().FirstOrDefault(x => x.Id == id && x.MemberId == memberId);
        }
    }
}
