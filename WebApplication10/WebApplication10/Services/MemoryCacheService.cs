using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication10.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _defaultCacheOptions;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _defaultCacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            var value = await factory();
            var options = expiration.HasValue
                ? new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
                : _defaultCacheOptions;

            _memoryCache.Set(key, value, options);
            return value;
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
    }
} 