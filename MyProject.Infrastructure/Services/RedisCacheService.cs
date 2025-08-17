using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MyProject.Core.Interfaces;

namespace MyProject.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (cachedData == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}