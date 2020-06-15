using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class TaskTypeRepository : GenericRepository<TaskType>
    {
        public TaskTypeRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        protected override IQueryable<TaskType> GetIncludes(IQueryable<TaskType> query) => query.Include(t => t.Project);

        public override TaskType LinkedCacheGetByName(string name) => LinkedCacheGetList().FirstOrDefault(p => p.Name == name);

        public override TaskType LinkedCacheGetById(int id) => LinkedCacheGetList().FirstOrDefault(x => x.Id == id); 

        public TaskType GetQueryWithIncludesById(int id) => GetQuery().FirstOrDefault(x => x.Id == id);
    }
}