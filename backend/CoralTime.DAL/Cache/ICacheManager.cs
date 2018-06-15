using System;
using System.Collections.Generic;

namespace CoralTime.DAL.Cache
{
    public interface ICacheManager
    {
        List<T> GetList<T>(string cacheKey) where T : class;

        T Get<T>(string cacheKey) where T : class;

        T Get<T>(string cacheKey, Func<T> funcCallBack, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void RemoveByKeys(IEnumerable<string> keys);

        void RemoveByKey(string key);

        #region Linked : Put, Clear

        void PutLinked<T>(string cacheKey, T item, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void PutLinkedList<T>(string key, List<T> data, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void ClearLinkedLists<T>();

        #endregion

        #region Single : Put, Clear

        void Put<T>(string cacheKey, T item, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;

        void PutList<T>(string key, List<T> data) where T : class;

        void Clear();

        #endregion
    }
}