using CoralTime.DAL.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class ReportsSettingsRepository : BaseRepository<ReportsSettings>
    {
        public ReportsSettingsRepository(AppDbContext context, IMemoryCache memoryCache, string userId)
            : base(context, memoryCache, userId) { }

        public List<ReportsSettings> GetEntitiesFromContex_ByMemberid(int memberId)
        {
            return GetQueryWithIncludes().Where(x => x.MemberId == memberId).ToList();
        }

        public ReportsSettings GetEntityFromContex_ByMemberidQueryname(int memberId, string queryName)
        {
            return GetQueryWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);
        }

        public List<ReportsSettings> GetEntitiesOutOfContex_ByMemberid(int memberId)
        {
            return GetQueryAsNoTrakingWithIncludes().Where(x => x.MemberId == memberId).ToList();
        }

        public ReportsSettings GetEntityOutOfContex_ByMemberidQueryId(int memberId, int? queryId)
        {
            return GetQueryAsNoTrakingWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.Id == queryId);
        }

        public ReportsSettings GetEntityOutOfContex_ByMemberidQueryname(int memberId, string queryName)
        {
            return GetQueryAsNoTrakingWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName);
        }

        //public ReportsSettings GetEntityOutOfContex_ByMemberidQuerynameQueryId(int memberId, string queryName, int? queryId)
        //{
        //    return GetQueryAsNoTrakingWithIncludes().FirstOrDefault(x => x.MemberId == memberId && x.QueryName == queryName && x.Id == queryId);
        //}
    }
}
