using LYRA.Server.Entities;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LYRA.Server.Data.Core.Caching
{
    /// <summary>
    /// Intercepts SaveChanges to synchronize the access policy cache in real time.
    /// Ensures that any change to AccessPolicyEntity, TrustedTouchpointEntity, or CompanyEntity
    /// is reflected in the denormalized cache.
    /// </summary>
    public class AccessPolicyCacheSyncInterceptor : SaveChangesInterceptor
    {
        private readonly ICachedAccessPolicyBuilder _builder;
        private readonly ICachedAccessPolicyService _cache;

        public AccessPolicyCacheSyncInterceptor(
            ICachedAccessPolicyBuilder builder,
            ICachedAccessPolicyService cache)
        {
            _builder = builder;
            _cache = cache;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context is null)
                return result;

            // ------------------- Collect entity changes -------------------

            var changedPolicies = context.ChangeTracker.Entries<AccessPolicyEntity>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            var deletedPolicies = context.ChangeTracker.Entries<AccessPolicyEntity>()
                .Where(e => e.State == EntityState.Deleted)
                .Select(e => e.Entity)
                .ToList();

            var updatedTouchpoints = context.ChangeTracker.Entries<TrustedTouchpointEntity>()
                .Where(e => e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            var updatedCompanies = context.ChangeTracker.Entries<CompanyEntity>()
                .Where(e => e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            // ------------------- Load affected policies based on dependencies -------------------

            var affectedFromTouchpoints = await context.Set<AccessPolicyEntity>()
                .Include(p => p.Caller).ThenInclude(t => t.Company)
                .Include(p => p.Target).ThenInclude(t => t.Company)
                .Where(p => updatedTouchpoints.Select(t => t.Id).Contains(p.CallerId) ||
                            updatedTouchpoints.Select(t => t.Id).Contains(p.TargetId))
                .ToListAsync(cancellationToken);

            var affectedFromCompanies = await context.Set<AccessPolicyEntity>()
                .Include(p => p.Caller).ThenInclude(t => t.Company)
                .Include(p => p.Target).ThenInclude(t => t.Company)
                .Where(p => updatedCompanies.Select(c => c.Id).Contains(p.Caller.CompanyId) ||
                            updatedCompanies.Select(c => c.Id).Contains(p.Target.CompanyId))
                .ToListAsync(cancellationToken);

            // Combine all affected policies (additions, updates, touchpoint/company-related)
            var affectedPolicies = changedPolicies
                .Concat(affectedFromTouchpoints)
                .Concat(affectedFromCompanies)
                .DistinctBy(p => p.Id)
                .ToList();

            // ------------------- Sync affected policies into cache -------------------

            foreach (var policy in affectedPolicies)
            {
                // Ensure we load navigation properties for policy builder
                context.Entry(policy).Reference(p => p.Caller).Query().Include(t => t.Company).Load();
                context.Entry(policy).Reference(p => p.Target).Query().Include(t => t.Company).Load();

                await BuildOrDeleteAsync(policy);
            }

            // ------------------- Remove deleted policies from cache -------------------

            foreach (var policy in deletedPolicies)
            {
                await _cache.DeleteByIdAsync(policy.Id);
            }

            return result;
        }

        /// <summary>
        /// Attempts to build a cached policy from a full access policy.
        /// If builder returns null (e.g. disabled/deleted/missing deps) — deletes from cache.
        /// </summary>
        private async Task BuildOrDeleteAsync(AccessPolicyEntity policy)
        {
            var cached = _builder.Build(policy);

            if (cached != null)
            {
                await _cache.UpsertAsync(cached);
            }
            else
            {
                await _cache.DeleteByIdAsync(policy.Id);
            }
        }
    }
}
