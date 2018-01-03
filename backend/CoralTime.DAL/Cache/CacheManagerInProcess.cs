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
        private IMemoryCache _cache;

        public CacheManagerInProcess(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

            // Add Types Cancelation Sources
            var cancellationTokenSources = CancelationTokenSources.GetCancelationTokenSourcesNames();
            cancellationTokenSources.ForEach(key => _cache.Set(key, new CancellationTokenSource()));
        }

        #region Linked.

        public void LinkedPut<T>(string cacheKey, T item, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
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
                tokens.ForEach(t=> cacheEntryOptions.AddExpirationToken(t));
                // Save data in cache.
                _cache.Set<T>(cacheKey, item, cacheEntryOptions);
            }
        }

        public void LinkedPutList<T>(string key, List<T> data, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
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
                tokens.ForEach(t => cacheEntryOptions.AddExpirationToken(t));

                // Save data in cache.
                _cache.Set(key, data, cacheEntryOptions);
            }
        }

        public void LinkedCacheClear<T>()
        {
            var listSources = CancelationTokenSources.GetNamesCancelationTokenSourcesForType<T>();
            lock (LockObject)
            {
                listSources.ForEach(ClearItemByCancellationToken);
            }
        }

        #endregion

        #region Single.

        public List<T> CachedListGet<T>(string cacheKey) where T : class
        {
            List<T> item;
            lock (LockObject)
            {
                item = _cache.Get(cacheKey) as List<T>;
            }

            return item;
        }

        public T Get<T>(string cacheKey, Func<T> funcCallBack, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
        {
            T item;
            lock (LockObject)
            {
                item = _cache.Get(cacheKey) as T;
            }

            if (item != null) { return item; }

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

                item = _cache.Set(cacheKey, item, cacheEntryOptions);
            }

            return item;
        }

        public T Get<T>(string cacheKey) where T : class
        {
            T item;
            lock (LockObject)
            {
                item = _cache.Get(cacheKey) as T;
            }

            return item;
        }

        public void Remove(string key)
        {
            lock (LockObject)
            {
                _cache.Remove(key);
            }
        }

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
                _cache.Set<T>(cacheKey, item, cacheEntryOptions);
            }
        }

        public void PutList<T>(string key, List<T> data) where T : class
        {
            lock (LockObject)
            {
                _cache.Set(key, data, TimeSpan.FromMinutes(SlidingExpirationTime));
            }
        }

        public void Remove(IEnumerable<string> keys)
        {
            lock (LockObject)
            {
                foreach (var key in keys)
                {
                    Remove(key);
                }
            }
        }

        public void Clear()
        {
            lock (LockObject)
            {
                _cache.Dispose();
                var options = new MemoryCacheOptions();
                _cache = new MemoryCache(options);
            }
        }

        #endregion

        private void ClearItemByCancellationToken (string cancellationTokenSourceName)
        {
            var cts = GetCancellationTokenSource(cancellationTokenSourceName);
            cts.Cancel();
            _cache.Remove(cancellationTokenSourceName);
            _cache.Set(cancellationTokenSourceName, new CancellationTokenSource());
        }

        private List<CancellationChangeToken> GetCancellationTokens<T>()
        {
            var listSources = CancelationTokenSources.GetNamesCancelationTokenSourcesForType<T>();
            return listSources.Select(GetCancellationChangeToken).ToList();
        }
        
        private CancellationTokenSource GetCancellationTokenSource(string cancellationTokenSourceName)
        {
            return _cache.Get<CancellationTokenSource>(cancellationTokenSourceName);
        }

        private CancellationChangeToken GetCancellationChangeToken(string cancellationTokenSourceName)
        {
            return new CancellationChangeToken(GetCancellationTokenSource(cancellationTokenSourceName).Token);
        }
    }
}