using LYRA.Server.Services.Verify.Interfaces;
using MILANO.Client.Interfaces;

namespace LYRA.Server.Services.Verify
{
    /// <summary>
    /// MILANO-based replay protection store.
    /// Keeps recently seen request IDs for a limited time window.
    /// </summary>
    public sealed class ReplayProtectionStore : IReplayProtectionStore
    {
        private readonly IMilanoCacheClient _cache;

        public ReplayProtectionStore(IMilanoCacheClient cache)
        {
            _cache = cache;
        }

        public async Task<bool> TryMarkAsUsedAsync(
            string callerSystemName,
            string targetSystemName,
            string requestId,
            TimeSpan ttl,
            CancellationToken ct = default)
        {
            var key = BuildKey(callerSystemName, targetSystemName, requestId);

            var existing = await _cache.GetAsync(key, ct);
            if (existing is not null)
                return false;

            await _cache.SetAsync(key, "1", ttl, ct);
            return true;
        }

        private static string BuildKey(string callerSystemName, string targetSystemName, string requestId)
        {
            return $"lyra:replay:{Normalize(callerSystemName)}:{Normalize(targetSystemName)}:{Normalize(requestId)}";
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToLowerInvariant();
        }
    }
}
