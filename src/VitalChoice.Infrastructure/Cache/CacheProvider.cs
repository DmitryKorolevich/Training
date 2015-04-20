using System;
using Microsoft.Framework.Caching.Memory;

namespace VitalChoice.Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
	    private readonly IMemoryCache memoryCache;

		private static readonly object padlock = new object();

		public CacheProvider(IMemoryCache memoryCache)
	    {
			this.memoryCache = memoryCache;
	    }

		public void SetItem(string key, object value, int minutes = 60)
		{
			lock (padlock)
			{
				memoryCache.Set(key, context => {
					context.SetAbsoluteExpiration(TimeSpan.FromMinutes(minutes));
					return value;
				});
			}
		}

	    public object GetItem(string key)
	    {
		    lock (padlock)
		    {
			    return memoryCache.Get(key);
		    }
	    }

		public T GetItem<T>(string key)
		{
			lock (padlock)
			{
				return memoryCache.Get<T>(key);
			}
		}

		public void Remove(string key)
		{
			lock (padlock)
			{
				memoryCache.Remove(key);
			}
		}
	}
}