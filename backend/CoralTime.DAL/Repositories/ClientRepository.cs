using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class ClientRepository : GenericRepository<Client>
    {
        public ClientRepository (AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<Client> GetIncludes(IQueryable<Client> query) => query.Include(x => x.Projects);

        #region LinkedCache.

        public override Client LinkedCacheGetByName(string name) => LinkedCacheGetList().FirstOrDefault(p => p.Name == name);

        public override Client LinkedCacheGetById(int id) =>LinkedCacheGetList().FirstOrDefault(c => c.Id == id);

        #endregion
    }
}