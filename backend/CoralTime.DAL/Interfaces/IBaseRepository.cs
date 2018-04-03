using System.Collections.Generic;
using System.Linq;

namespace CoralTime.DAL.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        //List<TEntity> GetList();

        //List<TEntity> GetPage<TKey>(int pageNumber, int pageSize, Expression<Func<TEntity, TKey>> orderBy, Expression<Func<TEntity, bool>> filter = null);

        //int GetCount();

        //#region Cache Methods.

        //string GenerateClientUniqueCacheKey(string userName);

        //TEntity GetByNameFromLinkedCache(string name);

        //TEntity GetByIdFromLinkedCache(int Id);

        //List<TEntity> GetCachedList();

        //List<TEntity> GetLinkedCachedList();

        //void ClearEntityCache();

        //void ClearLinkedCache();

        //#endregion

        IQueryable<TEntity> GetQueryWithIncludes();

        IQueryable<TEntity> GetQueryWithoutIncludes();

        IQueryable<TEntity> GetQueryAsNoTrackingWithIncludes();

        IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query);

        IQueryable<TEntity> GetQueryAsNoTraking();

        TEntity GetById(object id);

        void Insert(TEntity entity);

        void InsertRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void DeleteRange(IEnumerable<TEntity> entitiesToDelete);

        int ExecuteSqlCommand(string command, params object[] parameters);
    }
}