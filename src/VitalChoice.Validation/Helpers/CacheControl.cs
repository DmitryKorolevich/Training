using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Http;

namespace QRProject.Api.Helpers
{
    public class CacheControl
    {
        public static void SetCache(string cacheName, Guid userId, object obj)
        {
            //string key = string.Format("{0}{1}", cacheName, userId);
            //if (HttpContext.Current.Cache[key] != null)
            //{
            //    HttpContext.Current.Cache.Remove(key);
            //}
            //HttpContext.Current.Cache.Add
            //    (key,
            //     obj,
            //     null,
            //     DateTime.Now.Add(CacheConstants.CacheExpiration),
            //     Cache.NoSlidingExpiration,
            //     CacheItemPriority.Normal,
            //     null);
            throw new NotImplementedException();
        }

        public static bool GetFromCache<T>(string cacheName, Guid userId, out T result)
        {
            //var cacheItem = HttpContext.Current.Cache[string.Format("{0}{1}", cacheName, userId)];
            //if (cacheItem == null)
            //{
            //    result = default(T);
            //    return false;
            //}
            //result = (T)cacheItem;
            //return true;
            throw new NotImplementedException();
        }

        public static bool CheckCacheExists(string cacheName, Guid userId)
        {
            //return HttpContext.Current.Cache[string.Format("{0}{1}", cacheName, userId)] != null;
            throw new NotImplementedException();
        }
    }
}