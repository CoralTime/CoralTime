using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class ClientRepository : BaseRepository<Client>
    {
        public ClientRepository (AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }
        
        #region LinkedCache.

        public override IQueryable<Client> GetIncludes(IQueryable<Client> query)
        {
            return query.Include(x => x.Projects);
        }
        
        public override Client LinkedCacheGetByName(string name)
        {
            return LinkedCacheGetList().FirstOrDefault(p => p.Name == name);
        }

        public override Client LinkedCacheGetById(int id)
        {
            return LinkedCacheGetList().FirstOrDefault(c => c.Id == id);
        }

        #endregion
    }
}