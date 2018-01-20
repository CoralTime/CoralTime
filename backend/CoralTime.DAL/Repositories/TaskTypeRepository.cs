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


        public override TaskType LinkedCacheGetByName(string name)
        {
            return LinkedCacheGetList().FirstOrDefault(p => p.Name == name);
        }

        public override TaskType LinkedCacheGetById(int id)
        {
            return LinkedCacheGetList().FirstOrDefault(x => x.Id == id);
        }

        public override TaskType GetById(object id)
        {
            return LinkedCacheGetList().FirstOrDefault(z => z.Id == (int) id);
        }

        public override IQueryable<TaskType> GetIncludes(IQueryable<TaskType> query)
        {
            return query.Include(t => t.Project);
        }
    }
}