using System;
using System.Collections.Generic;

namespace CoralTime.DAL.Cache
{
    public interface ICacheManager
    {
        #region Linked.

        void LinkedPut<T>(string cacheKey, T item, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void LinkedPutList<T>(string key, List<T> data, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void LinkedCacheClear<T>();

        #endregion

        #region Single.

        List<T> CachedListGet<T>(string cacheKey) where T : class;

        T Get<T>(string cacheKey) where T : class;

        T Get<T>(string cacheKey, Func<T> funcCallBack, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void Put<T>(string cacheKey, T item, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void PutList<T>(string key, List<T> data) where T : class;

        void Remove(IEnumerable<string> keys);

        void Remove(string key);

        void Clear();

        #endregion
    }
}