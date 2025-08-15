using LYRA.Server.Entities;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Runtime.CompilerServices;

namespace LYRA.Server.Data.Core.Caching
{
    /// <summary>
    /// Intercepts SaveChanges to synchronize both physical (SQL) and memory (RAM) cache in real time.
    /// Ensures that any change to AccessPolicyEntity, TrustedTouchpointEntity, or CompanyEntity
    /// is reflected in the denormalized SQL cache and corresponding memory cache entry is invalidated.
    /// </summary>
    public class AccessPolicyCacheSyncInterceptor : SaveChangesInterceptor
    {
        private readonly ICachedAccessPolicyBuilder _builder;
        private readonly ICachedAccessPolicyService _cache;
        private readonly ICachedAccessPolicyMemoryService _memory;
		private static readonly ConditionalWeakTable<DbContext, SyncState> _state = new();

		public AccessPolicyCacheSyncInterceptor(
            ICachedAccessPolicyBuilder builder,
            ICachedAccessPolicyService cache,
            ICachedAccessPolicyMemoryService memory)
        {
            _builder = builder;
            _cache = cache;
            _memory = memory;
        }

		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
		{
			var ctx = eventData.Context;
			if (ctx is null) return new(result);

			var state = _state.GetOrCreateValue(ctx);
			state.Clear();

			var changed = ctx.ChangeTracker.Entries<AccessPolicyEntity>()
				.Where(e => e.State is EntityState.Added or EntityState.Modified)
				.Select(e => e.Entity.Id)
				.ToList();

			var deleted = ctx.ChangeTracker.Entries<AccessPolicyEntity>()
				.Where(e => e.State == EntityState.Deleted)
				.Select(e => e.Entity.Id)
				.ToList();

			state.PolicyIdsToRebuild.UnionWith(changed);
			state.PolicyIdsToDelete.UnionWith(deleted);

			var touchIds = ctx.ChangeTracker.Entries<TrustedTouchpointEntity>()
				.Where(e => e.State == EntityState.Modified)
				.Select(e => e.Entity.Id)
				.ToHashSet();

			var compIds = ctx.ChangeTracker.Entries<CompanyEntity>()
				.Where(e => e.State == EntityState.Modified)
				.Select(e => e.Entity.Id)
				.ToHashSet();

			if (touchIds.Count > 0 || compIds.Count > 0)
			{
				var affected = ctx.Set<AccessPolicyEntity>()
					.Where(p => touchIds.Contains(p.CallerId) || touchIds.Contains(p.TargetId)
							 || compIds.Contains(p.Caller.CompanyId) || compIds.Contains(p.Target.CompanyId))
					.Select(p => p.Id)
					.ToList();

				state.PolicyIdsToRebuild.UnionWith(affected);
			}

			foreach (var e in ctx.ChangeTracker.Entries<AccessPolicyEntity>().Where(e => e.State == EntityState.Modified))
			{
				var oldCaller = e.OriginalValues.GetValue<string>(nameof(AccessPolicyEntity.CallerSystemName));
				var oldTarget = e.OriginalValues.GetValue<string>(nameof(AccessPolicyEntity.TargetSystemName));
				if (!string.IsNullOrWhiteSpace(oldCaller) && !string.IsNullOrWhiteSpace(oldTarget))
					state.MemoryKeysToInvalidate.Add((oldCaller, oldTarget));
			}

			return new(result);
		}

		public override async ValueTask<int> SavedChangesAsync(
			SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
		{
			var ctx = eventData.Context;
			if (ctx is null || result <= 0) return result;

			if (!_state.TryGetValue(ctx, out var state)) return result;

			var policies = await ctx.Set<AccessPolicyEntity>()
				.Where(p => state.PolicyIdsToRebuild.Contains(p.Id))
				.Include(p => p.Caller).ThenInclude(t => t.Company)
				.Include(p => p.Target).ThenInclude(t => t.Company)
				.AsNoTracking()
				.ToListAsync(ct);

			var toUpsert = new List<CachedAccessPolicyEntity>(policies.Count);
			foreach (var p in policies)
			{
				var built = _builder.Build(p);
				if (built != null) toUpsert.Add(built);
				else state.PolicyIdsToDelete.Add(p.Id);
			}

			if (toUpsert.Count > 0) await _cache.UpsertManyAsync(toUpsert, ct);
			if (state.PolicyIdsToDelete.Count > 0) await _cache.DeleteManyAsync(state.PolicyIdsToDelete, ct);

			var newKeys = toUpsert.Select(x => (x.CallerSystemName, x.TargetSystemName));
			_memory.InvalidateMany(state.MemoryKeysToInvalidate.Concat(newKeys));

			state.Clear();
			return result;
		}

		private sealed class SyncState
		{
			public HashSet<Guid> PolicyIdsToRebuild { get; } = new();

			public HashSet<Guid> PolicyIdsToDelete { get; } = new();

			public HashSet<(string caller, string target)> MemoryKeysToInvalidate { get; } = new();

			public void Clear()
			{
				PolicyIdsToRebuild.Clear();
				PolicyIdsToDelete.Clear();
				MemoryKeysToInvalidate.Clear();
			}
		}
	}
}
