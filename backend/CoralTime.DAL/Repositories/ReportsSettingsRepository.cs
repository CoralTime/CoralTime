using CoralTime.DAL.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class ReportsSettingsRepository : BaseRepository<ReportsSettings>
    {
        public ReportsSettingsRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        public ReportsSettings GetQueryByMemberIdWithIncludes(int memberId)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.MemberId == memberId);
        }

        public ReportsSettings GetQueryByMemberIdAsNoTrakingWithIncludes(int memberId)
        {
            return GetQueryAsNoTrakingWithIncludes().FirstOrDefault(x => x.MemberId == memberId);
        }
    }
}
