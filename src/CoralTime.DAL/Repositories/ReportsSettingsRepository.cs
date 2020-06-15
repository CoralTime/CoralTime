using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using CoralTime.DAL.Models.ReportsSettings;

namespace CoralTime.DAL.Repositories
{
    public class ReportsSettingsRepository : GenericRepository<ReportsSettings>
    {
        public ReportsSettingsRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        protected override IQueryable<ReportsSettings> GetIncludes(IQueryable<ReportsSettings> query) => query.Include(x => x.Member);

        public List<ReportsSettings> GetQueryByMemberId(int memberId) => GetQuery().Where(x => x.MemberId == memberId).ToList();
        
        public ReportsSettings GetQueryByMemberIdQueryId(int memberId, int? queryId) => GetQuery().FirstOrDefault(x => x.MemberId == memberId && x.Id == queryId);

        public ReportsSettings GetQueryByMemberIdQueryName(int memberId, string queryName) => GetQuery().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);

        public List<ReportsSettings> LinkedCacheGetByMemberId(int memberId) => LinkedCacheGetList().Where(x => x.MemberId == memberId).ToList();
    }
}
