using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CoralTime.DAL.Cache
{
    public class CacheManagerInProcess : ICacheManager
    {
        private const int SlidingExpirationTime = 10000; //TODO: will be refactored?
        private const int AbsoluteExpirationTime = 10000; //TODO: Can this be removed?
        private static readonly object LockObject = new object();
        private IMemoryCache _memoryCache;

        public CacheManagerInProcess(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            CancelationTokenSources.GetCancelationTokenSourcesNames.ForEach(key => _memoryCache.Set(key, new CancellationTokenSource()));
        }

        public List<T> GetList<T>(string cacheKey) where T : class
        {
            List<T> item;
            lock (LockObject)
            {
                item = _memoryCache.Get(cacheKey) as List<T>;
            }

            return item;
        }

        //TODO rewrite!
        public T Get<T>(string cacheKey, Func<T> funcCallBack, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
        {
            T item;

            lock (LockObject)
            {
                item = _memoryCache.Get(cacheKey) as T;
            }

            if (item != null)
            {
                return item;
            }

            item = funcCallBack();

            lock (LockObject)
            {
                var slidingExpirationValue = slidingExpiration ?? TimeSpan.FromMinutes(SlidingExpirationTime);
                var absoluteExpirationTimeValue = absoluteExpiration ?? DateTime.Now + TimeSpan.FromMinutes(AbsoluteExpirationTime);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = absoluteExpirationTimeValue,
                    SlidingExpiration = slidingExpirationValue
                };

                item = _memoryCache.Set(cacheKey, item, cacheEntryOptions);
            }

            return item;
        }

        public T Get<T>(string cacheKey) where T : class
        {
            T item;
            lock (LockObject)
            {
                item = _memoryCache.Get(cacheKey) as T;
            }

            return item;
        }

        public void RemoveByKey(string key)
        {
            lock (LockObject)
            {
                _memoryCache.Remove(key);
            }
        }

        public void RemoveByKeys(IEnumerable<string> keys)
        {
            lock (LockObject)
            {
                foreach (var key in keys)
                {
                    RemoveByKey(key);
                }
            }
        }

        #region Linked.

        public void PutLinked<T>(string cacheKey, T entity, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
        {
            lock (LockObject)
            {
                var slidingExpirationValue = slidingExpiration ?? TimeSpan.FromMinutes(SlidingExpirationTime);
                var absoluteExpirationTimeValue = absoluteExpiration ?? DateTime.Now + TimeSpan.FromMinutes(AbsoluteExpirationTime);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = absoluteExpirationTimeValue,
                    SlidingExpiration = slidingExpirationValue
                };

                var tokens = GetCancellationTokens<T>();
                tokens.ForEach(token => cacheEntryOptions.AddExpirationToken(token));
                
                // Save data in cache.
                _memoryCache.Set(cacheKey, entity, cacheEntryOptions);
            }
        }

        public void PutLinkedList<T>(string cacheKey, List<T> linkedList, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
        {
            lock (LockObject)
            {
                var slidingExpirationValue = slidingExpiration ?? TimeSpan.FromMinutes(SlidingExpirationTime);
                var absoluteExpirationTimeValue = absoluteExpiration ?? DateTime.Now + TimeSpan.FromMinutes(AbsoluteExpirationTime);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = absoluteExpirationTimeValue,
                    SlidingExpiration = slidingExpirationValue
                };

                var tokens = GetCancellationTokens<T>();
                tokens.ForEach(token => cacheEntryOptions.AddExpirationToken(token));

                // Save data in cache.
                _memoryCache.Set(cacheKey, linkedList, cacheEntryOptions);
            }
        }

        public void ClearLinkedLists<T>()
        {
            var listSources = CancelationTokenSources.GetNamesCancelationTokenSourcesForType<T>();
            lock (LockObject)
            {
                listSources.ForEach(ClearItemByCancellationToken);
            }
        }

        #endregion

        #region Single.

        public void Put<T>(string cacheKey, T item, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
        {
            lock (LockObject)
            {
                var slidingExpirationValue = slidingExpiration ?? TimeSpan.FromMinutes(SlidingExpirationTime);
                var absoluteExpirationTimeValue = absoluteExpiration ?? DateTime.Now + TimeSpan.FromMinutes(AbsoluteExpirationTime);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = absoluteExpirationTimeValue,
                    SlidingExpiration = slidingExpirationValue
                };

                // Save data in cache.
                _memoryCache.Set(cacheKey, item, cacheEntryOptions);
            }
        }

        public void PutList<T>(string key, List<T> data) where T : class
        {
            lock (LockObject)
            {
                _memoryCache.Set(key, data, TimeSpan.FromMinutes(SlidingExpirationTime));
            }
        }

        public void Clear()
        {
            lock (LockObject)
            {
                _memoryCache.Dispose();
                var options = new MemoryCacheOptions();
                _memoryCache = new MemoryCache(options);
            }
        }

        #endregion

        private void ClearItemByCancellationToken (string cancellationTokenSourceName)
        {
            var cts = GetCancellationTokenSource(cancellationTokenSourceName);
            cts.Cancel();
            _memoryCache.Remove(cancellationTokenSourceName);
            _memoryCache.Set(cancellationTokenSourceName, new CancellationTokenSource());
        }

        private List<CancellationChangeToken> GetCancellationTokens<T>()
        {
            var listSources = CancelationTokenSources.GetNamesCancelationTokenSourcesForType<T>();
            return listSources.Select(GetCancellationChangeToken).ToList();
        }
        
        private CancellationTokenSource GetCancellationTokenSource(string cancellationTokenSourceName)
        {
            return _memoryCache.Get<CancellationTokenSource>(cancellationTokenSourceName);
        }

        private CancellationChangeToken GetCancellationChangeToken(string cancellationTokenSourceName)
        {
            return new CancellationChangeToken(GetCancellationTokenSource(cancellationTokenSourceName).Token);
        }
    }
}