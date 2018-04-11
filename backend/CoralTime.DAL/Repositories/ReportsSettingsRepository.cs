using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class ReportsSettingsRepository : BaseRepository<ReportsSettings>
    {
        public ReportsSettingsRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        public override IQueryable<ReportsSettings> GetIncludes(IQueryable<ReportsSettings> query)
        {
            return query
                .Include(x => x.Member);
        }

        public List<ReportsSettings> GetEntitiesFromContex_ByMemberId(int memberId)
        {
            return GetQueryWithIncludes().Where(x => x.MemberId == memberId).ToList();
        }

        public ReportsSettings GetEntityFromContex_ByMemberIdQueryId(int memberId, int? queryId)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.Id == queryId);
        }

        public ReportsSettings GetEntityFromContext_ByMemberIdQueryName(int memberId, string queryName)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);
        }

        public List<ReportsSettings> LinkedCacheGetByMemberId(int memberId)
        {
            return LinkedCacheGetList().Where(x => x.MemberId == memberId).ToList();
        }
    }
}
