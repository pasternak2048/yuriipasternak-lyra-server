using LYRA.Server.Data.LyraDb;
using LYRA.Server.Entities;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services.AccessPolicy
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
            var policies = await _mainDb.AccessPolicies
                .Include(p => p.Caller)
                    .ThenInclude(t => t.Company)
                .Include(p => p.Target)
                    .ThenInclude(t => t.Company)
                .AsNoTracking()
                .ToListAsync();

            var cached = policies
                .Select(p => _builder.Build(p))
                .Where(p => p != null)
                .Cast<CachedAccessPolicyEntity>()
                .ToList();

            await _cacheService.ReplaceAllAsync(cached);
        }
    }
}
