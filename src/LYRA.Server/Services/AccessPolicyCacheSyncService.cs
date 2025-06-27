using LYRA.Server.Data.LyraDb;
using LYRA.Server.Entities;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services
{
    /// <summary>
    /// Synchronizes access policies from the main database into the cache database.
    /// Filters out disabled, inactive, or deleted entries before caching.
    /// </summary>
    public class AccessPolicyCacheSyncService : IAccessPolicyCacheSyncService
    {
        private readonly LyraDbContext _mainDb;
        private readonly ICachedAccessPolicyBuilder _builder;
        private readonly ICachedAccessPolicyService _cacheService;

        public AccessPolicyCacheSyncService(
            LyraDbContext mainDb,
            ICachedAccessPolicyBuilder builder,
            ICachedAccessPolicyService cacheService)
        {
            _mainDb = mainDb;
            _builder = builder;
            _cacheService = cacheService;
        }

        /// <inheritdoc/>
        public async Task SyncFromDbAsync()
        {
            // Load all access policies including necessary navigation properties
            var policies = await _mainDb.AccessPolicies
                .Include(p => p.Caller)
                    .ThenInclude(t => t.Company)
                .Include(p => p.Target)
                    .ThenInclude(t => t.Company)
                .AsNoTracking()
                .ToListAsync();

            // Transform and filter
            var cached = policies
                .Select(p => _builder.Build(p))
                .Where(p => p != null)
                .Cast<CachedAccessPolicyEntity>()
                .ToList();

            // Replace cache
            await _cacheService.ReplaceAllAsync(cached);
        }
    }
}
