using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Cache;

namespace WebAPI.Net6.BaseRepositoryGenerics.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            var value = _cache.GetString(key);

            if (value != null)
                return JsonConvert.DeserializeObject<T>(value);

            return default;
        }

        public T Set<T>(string key, T value)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            _cache.SetString(key, JsonConvert.SerializeObject(value), options);

            return value;
        }

        public T Set<T>(string key, T value, int minute)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(minute),
            };

            _cache.SetString(key, JsonConvert.SerializeObject(value), options);

            return value;
        }
    }
}
