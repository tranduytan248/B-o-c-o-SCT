using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace TSFramework.Core.Members.Caching
{
    public class CacheLayer
    {
        private const double AppCacheDuration = 60.0;
        protected virtual string[] MasterCacheKeyArray { get; }

        private string GetCacheKey(string cacheKey)
        {
            return string.Concat(MasterCacheKeyArray[0], "-", cacheKey);
        }

        protected object GetCacheItem(string rawKey)
        {
            return HttpRuntime.Cache[GetCacheKey(rawKey)];
        }

        protected void AddCacheItem(string rawKey, object value)
        {
            if (value == null) return;
            var dataCache = HttpRuntime.Cache;

            // Make sure MasterCacheKeyArray[0] is in the cache - if not, add it
            if (dataCache[MasterCacheKeyArray[0]] == null) dataCache[MasterCacheKeyArray[0]] = DateTime.Now;
            // Add a CacheDependency
            var dependency = new CacheDependency(null, new[] {MasterCacheKeyArray[0]});
            dataCache.Insert(GetCacheKey(rawKey), value, dependency, DateTime.Now.AddMinutes(AppCacheDuration),
                Cache.NoSlidingExpiration);
        }

        protected void InvalidateCache()
        {
            // Remove the cache dependency
            if (MasterCacheKeyArray.Length <= 0) return;
            foreach (var masterCache in MasterCacheKeyArray)
                HttpRuntime.Cache.Remove(masterCache);
        }

        public void InvalidateCache(string rawKey)
        {
            // Remove the cache dependency
            HttpRuntime.Cache.Remove(rawKey);
        }

        public void InvalidateAllCache()
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            var cacheItems = new Dictionary<string, object>();

            while (enumerator.MoveNext())
                if (enumerator.Key != null)
                    cacheItems.Add(enumerator.Key?.ToString(), enumerator.Value);

            foreach (var key in cacheItems.Keys)
                HttpRuntime.Cache.Remove(key);
        }
    }
}