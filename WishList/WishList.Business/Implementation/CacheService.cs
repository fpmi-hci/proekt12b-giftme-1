using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using WishList.Business.IServices;

namespace WishList.Business.Implementation
{
    public class CacheService : ICacheService
    {
        private readonly IConfiguration _configuration;

        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache, IConfiguration configuration)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public void AddToCache<T>(T o, string key)
        {
            _cache.Set(key, o, TimeSpan.FromHours(24));
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void RemoveFromCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
