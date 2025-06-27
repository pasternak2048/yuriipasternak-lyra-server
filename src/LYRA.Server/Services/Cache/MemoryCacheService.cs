using LYRA.Server.Services.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LYRA.Server.Services.Cache
{
    /// <summary>
    /// Default in-memory implementation of <see cref="ICacheService"/> using <see cref="IMemoryCache"/>.
    /// Suitable for local scenarios or lightweight caching.
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _defaultOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService"/> with default TTL.
        /// </summary>
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _defaultOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
        }

        /// <inheritdoc/>
        public Task<T?> GetAsync<T>(string key)
        {
            var value = _memoryCache.TryGetValue(key, out var result)
                ? (T?)result
                : default;

            return Task.FromResult(value);
        }

        /// <inheritdoc/>
        public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            var options = ttl.HasValue
                ? new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }
                : _defaultOptions;

            _memoryCache.Set(key, value!, options);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ClearAsync()
        {
            // No built-in method, so forcefully compact all entries
            if (_memoryCache is MemoryCache concrete)
                concrete.Compact(1.0);

            return Task.CompletedTask;
        }
    }
}
