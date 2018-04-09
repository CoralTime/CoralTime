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

        public List<ReportsSettings> GetEntityOutOfContex_ByMemberId(int memberId)
        {
            return GetQueryAsNoTrackingWithIncludes().Where(x => x.MemberId == memberId).ToList();
        }

        public List<ReportsSettings> GetEntitiesFromContex_ByMemberId(int memberId)
        {
            return GetQueryWithIncludes().Where(x => x.MemberId == memberId).ToList();
        }

        public ReportsSettings GetEntityOutOfContex_ByMemberIdQueryId(int memberId, int? queryId)
        {
            return GetQueryAsNoTrackingWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.Id == queryId);
        }

        public ReportsSettings GetEntityOutOfContex_ByMemberIdQueryName(int memberId, string queryName)
        {
            return GetQueryAsNoTrackingWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);
        }

        public ReportsSettings GetEntityFromContext_ByMemberIdQueryName(int memberId, string queryName)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);
        }

        public List<ReportsSettings> LinkedCacheGetByMemberId(int memberId)
        {
            return LinkedCacheGetList().Where(x => x.MemberId == memberId).ToList();
        }

        public ReportsSettings LinkedCacheGetByMemberIdQueryName(int memberId, string queryName)
        {
            return LinkedCacheGetList().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);
        }
    }
}
