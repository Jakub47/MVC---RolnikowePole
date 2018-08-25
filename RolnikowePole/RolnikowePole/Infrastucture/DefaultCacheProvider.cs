using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace RolnikowePole.Infrastucture
{
    public class DefaultCacheProvider : ICacheProvider
    {
        //Get our current cache from app!
        private Cache cache { get { return HttpContext.Current.Cache; } }

        //Return cache
        public object Get(string key)
        {
            return cache[key];
        }

        //Create cache with given key
        public void Set(string key, object data, int cacheTime)
        {
            var expirationTime = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            cache.Insert(key, data, null, expirationTime, Cache.NoSlidingExpiration);
        }

        //Check if cache with given key exists
        public bool IsSet(string key)
        {
            return (cache[key] != null);
        }

        //Remove Cache with key
        public void Invalidate(string key)
        {
            cache.Remove(key);
        }
        
    }
}