using LYRA.Server.Entities;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services
{
    /// <inheritdoc/>
    public class CachedAccessPolicyMemoryService : ICachedAccessPolicyMemoryService
    {
        private readonly ICacheService _cache;
        private readonly ICachedAccessPolicyService _persistent;
        private readonly ILogger<CachedAccessPolicyMemoryService> _logger;
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);

        public CachedAccessPolicyMemoryService(
            ICacheService cache,
            ICachedAccessPolicyService persistent,
            ILogger<CachedAccessPolicyMemoryService> logger)
        {
            _cache = cache;
            _persistent = persistent;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CachedAccessPolicyEntity?> GetAsync(string caller, string target, string context, string operation)
        {
            var key = BuildKey(caller, target, context, operation);

            var cached = await _cache.GetAsync<CachedAccessPolicyEntity>(key);
            if (cached != null)
                return cached;

            cached = await _persistent.FindAsync(caller, target, context, operation);
            if (cached != null)
                await _cache.SetAsync(key, cached, DefaultTtl);

            return cached;
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(string caller, string target, string context, string operation)
        {
            var key = BuildKey(caller, target, context, operation);
            var cached = await _persistent.FindAsync(caller, target, context, operation);

            if (cached != null)
                await _cache.SetAsync(key, cached, DefaultTtl);
            else
                await _cache.RemoveAsync(key);
        }

        /// <inheritdoc/>
        public async void Invalidate(string caller, string target, string context, string operation)
        {
            var key = BuildKey(caller, target, context, operation);
            await _cache.RemoveAsync(key);
        }

        //TODO: load memory cache on start
        /// <inheritdoc/>
        public async Task WarmupAsync()
        {
            var all = await _persistent.GetAllAsync();

            foreach (var policy in all)
            {
                var key = BuildKey(policy.CallerSystemName, policy.TargetSystemName, policy.Context, policy.Operation);
                await _cache.SetAsync(key, policy, DefaultTtl);
            }
        }

        private static string BuildKey(string caller, string target, string context, string operation)
            => $"{caller}::{target}::{context}::{operation}".ToLowerInvariant();
    }
}
