using CoralTime.Common.Exceptions;
using CoralTime.DAL.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using CoralTime.DAL.Models.LogChanges;

namespace CoralTime.DAL.Repositories
{
    public abstract class GenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        private readonly ICacheManager _cacheManager;

        private static readonly object LockCacheObject = new object();

        private readonly string _userId;

        protected GenericRepository(AppDbContext appDbContext, IMemoryCache memoryCache, string userId)
        {
            _dbContext = appDbContext;
            _dbSet = _dbContext.Set<TEntity>();
            _cacheManager = CacheMemoryFactory.CreateCacheMemory(memoryCache);
              
            _userId = userId;
        }

        private static string GenerateCacheKey() => $"{ typeof(TEntity).Name}_CacheKey";

        #region GetQuery.

        protected abstract IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query);

        // Use asNoTracking = TRUE for POST / PATCH type queries (if you want get entity from db and update their properties)
        public IQueryable<TEntity> GetQuery(bool withIncludes = true, bool asNoTracking = false)
        {
            var localDbSet = withIncludes
                ? GetIncludes(_dbSet)
                : _dbSet;

            if (asNoTracking)
            {
                localDbSet = localDbSet.AsNoTracking();
            }

            return localDbSet;
        }

        #endregion

        #region LinkedCache. 

        public virtual TEntity LinkedCacheGetByName(string name) => throw new CoralTimeDangerException("You forget override \"LinkedCacheGetByName\"");

        public virtual TEntity LinkedCacheGetById(int id) => throw new CoralTimeDangerException("You forget override \"LinkedCacheGetById\"");

        public virtual List<TEntity> LinkedCacheGetList()
        {
            try
            {
                var key = GenerateCacheKey();
                var items = _cacheManager.GetList<TEntity>(key);
                if (items == null)
                {
                    lock (LockCacheObject)
                    {
                        items = _cacheManager.GetList<TEntity>(key);
                        if (items == null)
                        {
                            items = GetQuery(asNoTracking:true).ToList();
                            _cacheManager.PutLinkedList(key, items);
                        }
                    }
                }

                return items;
            }
            catch (Exception seq)
            {
                throw new CoralTimeDangerException(seq.Message, seq);
            }
        }

        public virtual void LinkedCacheClear() => _cacheManager.ClearLinkedLists<TEntity>();

        #endregion

        #region Single Cache.

        //protected int DefaultCacheTime { get; set; } = 800;

        //public virtual List<TEntity> GetCachedList(Func<List<TEntity>> getListFunc)
        //{
        //    string key = GenerateClientUniqueCacheKey();
        //    return CacheManager.Get(key, getListFunc);
        //}


        //public string GenerateClientUniqueCacheKey(string userName)
        //{
        //    var entityName = typeof(T).Name;
        //    var key = $"{userName}_{entityName}_CacheKey";
        //    return key;
        //}

        //public void ClearEntityCache()
        //{
        //    var key = GenerateCacheKey();
        //    _cacheManager.RemoveByKey(key);
        //}

        //public void SingleCacheClearByKey(string key) =>  _cacheManager.RemoveByKey(key);

        #endregion

        #region CRUD

        public virtual TEntity GetById(object id) =>  _dbSet.Find(id);

        public virtual void Insert(TEntity entity, string userId = null)
        {
            if (entity is ILogChanges entityILogChange)
            {
                SetInfoAboutUserThatCratedEntity(entityILogChange, userId);
                SetInfoAboutUserThatUpdatedEntity(entityILogChange, userId);

                entity = (TEntity)entityILogChange;
            }

            _dbSet.Add(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities, string userId = null)
        {
            if (entities is IEnumerable<ILogChanges> entitiesILogChange)
            {
                foreach (var entityILogChange in entitiesILogChange)
                {
                    SetInfoAboutUserThatCratedEntity(entityILogChange, userId);
                    SetInfoAboutUserThatUpdatedEntity(entityILogChange, userId);
                }

                entities = (IEnumerable<TEntity>)entitiesILogChange;
            }

            _dbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity, string userId = null)
        {
            if (entity is ILogChanges entityILogChange)
            {
                SetInfoAboutUserThatUpdatedEntity(entityILogChange, userId);

                entity = (TEntity)entityILogChange;
            }

            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, string userId = null)
        {
            if (entities is IEnumerable<ILogChanges> entitiesILogChange)
            {
                foreach (var entityILogChange in entitiesILogChange)
                {
                    SetInfoAboutUserThatUpdatedEntity(entityILogChange, userId);
                }

                entities = (IEnumerable<TEntity>)entitiesILogChange;
            }

            _dbSet.UpdateRange(entities);
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }

            _dbSet.Remove(entityToDelete);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entitiesToDelete)
        {
            foreach (var entityToDelete in entitiesToDelete)
            {
                if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
                {
                    _dbSet.Attach(entityToDelete);
                }

                _dbSet.Remove(entityToDelete);
            }
        }

        #endregion

        public int ExecuteSqlCommand(string command, params object[] parameters) => _dbContext.Database.ExecuteSqlCommand(command, parameters);

        private void SetInfoAboutUserThatCratedEntity(ILogChanges entityILogChange, string userId = null)
        {
            entityILogChange.CreatorId = userId ?? _userId;
            entityILogChange.CreationDate = DateTime.Now;
        }

        private void SetInfoAboutUserThatUpdatedEntity(ILogChanges entityILogChange, string userId = null)
        {
            entityILogChange.LastUpdateDate = DateTime.Now;
            entityILogChange.LastEditorUserId = userId ?? _userId;
        }
    }
}