using System.Linq;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Repositories
{
    public class ProjectRoleRepository : BaseRepository<ProjectRole>
    {
        public ProjectRoleRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        public int GetManagerRoleId()
        {
            return LinkedCacheGetList().FirstOrDefault(z => z.Name == Constants.ProjectRoleManager).Id;
        }

        public int GetMemberRoleId()
        {
            return LinkedCacheGetList().FirstOrDefault(z => z.Name == Constants.ProjectRoleMember).Id;
        }

        public ProjectRole GetMemberRole()
        {
            var memberRole = LinkedCacheGetList().FirstOrDefault(z => z.Name == Constants.ProjectRoleMember);
            return memberRole;
        }
    }
}