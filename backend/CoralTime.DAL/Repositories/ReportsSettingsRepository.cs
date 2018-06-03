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

        public override IQueryable<ReportsSettings> GetIncludes(IQueryable<ReportsSettings> query) => query.Include(x => x.Member);

        public List<ReportsSettings> GetQueryByMemberId(int memberId) => GetQueryWithIncludes().Where(x => x.MemberId == memberId).ToList();
        
        public ReportsSettings GetQueryByMemberIdQueryId(int memberId, int? queryId) => GetQueryWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.Id == queryId);

        public ReportsSettings GetQueryByMemberIdQueryName(int memberId, string queryName) => GetQueryWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);

        public List<ReportsSettings> LinkedCacheGetByMemberId(int memberId) => LinkedCacheGetList().Where(x => x.MemberId == memberId).ToList();
    }
}
