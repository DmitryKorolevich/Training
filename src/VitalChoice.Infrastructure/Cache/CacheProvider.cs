using System;
using Microsoft.Extensions.Caching.Memory;

namespace VitalChoice.Infrastructure.Cache
{
    public class CacheProvider : ICacheProvider
    {
	    private readonly IMemoryCache _memoryCache;

		private static readonly object Padlock = new object();

		public CacheProvider(IMemoryCache memoryCache)
	    {
			this._memoryCache = memoryCache;
	    }

		public void SetItem(string key, object value, int minutes = 60)
		{
		    lock (Padlock)
		    {
		        _memoryCache.Set(key, value, new MemoryCacheEntryOptions
		        {
		            AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddMinutes(minutes))
		        });
		    }
		}

	    public object GetItem(string key)
	    {
		    lock (Padlock)
		    {
			    return _memoryCache.Get(key);
		    }
	    }

		public T GetItem<T>(string key)
		{
			lock (Padlock)
			{
				return _memoryCache.Get<T>(key);
			}
		}

		public void Remove(string key)
		{
			lock (Padlock)
			{
				_memoryCache.Remove(key);
			}
		}
	}
}