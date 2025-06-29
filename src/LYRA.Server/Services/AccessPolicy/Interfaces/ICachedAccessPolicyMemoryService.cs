using LYRA.Server.Entities;

namespace LYRA.Server.Services.AccessPolicy.Interfaces
{
    /// <summary>
    /// Hybrid cache for fast access to CachedAccessPolicyEntity using RAM (IMemoryCache)
    /// with fallback to persistent SQL-based cache.
    /// </summary>
    public interface ICachedAccessPolicyMemoryService
    {
        /// <summary>
        /// Tries to get cached policy from RAM or persistent storage.
        /// </summary>
        Task<CachedAccessPolicyEntity?> GetAsync(string caller, string target, string context);

        /// <summary>
        /// Invalidates the cache for a given policy.
        /// </summary>
        void Invalidate(string caller, string target, string context);

        /// <summary>
        /// Forcefully refreshes the RAM cache entry.
        /// </summary>
        Task RefreshAsync(string caller, string target, string context);

        /// <summary>
        /// Preloads all policies from persistent cache into memory.
        /// </summary>
        Task WarmupAsync();
    }
}
