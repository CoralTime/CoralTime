using CoralTime.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace CoralTime.DAL.Repositories
{
    public class TaskTypeRepository : BaseRepository<TaskType>
    {
        public TaskTypeRepository(AppDbContext context, IMemoryCache memoryCache, string userId) 
            : base(context, memoryCache, userId) { }

        public override TaskType LinkedCacheGetByName(string name) => LinkedCacheGetList().FirstOrDefault(p => p.Name == name);

        public override TaskType LinkedCacheGetById(int id) => LinkedCacheGetList().FirstOrDefault(x => x.Id == id); 

        public override IQueryable<TaskType> GetIncludes(IQueryable<TaskType> query) => query.Include(t => t.Project);

        public override TaskType GetQueryWithIncludesById(int id) => GetQueryWithIncludes().FirstOrDefault(x => x.Id == id);
    }
}