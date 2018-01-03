using Microsoft.Extensions.Caching.Memory;

namespace CoralTime.DAL.Cache
{
    public class CacheMemoryFactory
    {
        private static ICacheManager _cacheManager;
        private static IMemoryCache _memoryCache;

        public static ICacheManager CreateCacheMemory(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cacheManager = _cacheManager ?? new CacheManagerInProcess(_memoryCache);
            return _cacheManager;
        }

        public static ICacheManager LastCreatedCacheManager => _cacheManager;
    }
}
