using LYRA.Server.Data.LyraCachedDb;
using LYRA.Server.Entities;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services.AccessPolicy.Stores
{
	/// <summary>
	/// Physical SQL-based store for cached access policies.
	/// Serves as a source of truth for MILANO-distributed memory cache.
	/// </summary>
	public sealed class CachedAccessPolicyStore : ICachedAccessPolicyStore
	{
		private readonly LyraCachedDbContext _db;

		public CachedAccessPolicyStore(LyraCachedDbContext db) => _db = db;

		/// <inheritdoc />
		public async Task UpsertAsync(CachedAccessPolicyEntity item, CancellationToken ct = default)
		{
			var existing = await _db.CachedAccessPolicies
				.AsTracking()
				.FirstOrDefaultAsync(x => x.Id == item.Id, ct);

			if (existing is null)
				await _db.CachedAccessPolicies.AddAsync(item, ct);
			else
				_db.Entry(existing).CurrentValues.SetValues(item);

			await _db.SaveChangesAsync(ct);
		}

		/// <inheritdoc />
		public async Task UpsertManyAsync(IEnumerable<CachedAccessPolicyEntity> items, CancellationToken ct = default)
		{
			var list = items.ToList();
			if (list.Count == 0) return;

			var ids = list.Select(x => x.Id).ToList();

			var existing = await _db.CachedAccessPolicies
				.Where(x => ids.Contains(x.Id))
				.ToListAsync(ct);

			var existingMap = existing.ToDictionary(x => x.Id);

			var toAdd = new List<CachedAccessPolicyEntity>(Math.Max(8, list.Count));
			foreach (var it in list)
			{
				if (existingMap.TryGetValue(it.Id, out var ex))
					_db.Entry(ex).CurrentValues.SetValues(it);
				else
					toAdd.Add(it);
			}

			if (toAdd.Count > 0)
				await _db.CachedAccessPolicies.AddRangeAsync(toAdd, ct);

			await _db.SaveChangesAsync(ct);
		}

		/// <inheritdoc />
		public async Task DeleteByIdAsync(Guid id, CancellationToken ct = default)
		{
			await _db.CachedAccessPolicies
				.Where(x => x.Id == id)
				.ExecuteDeleteAsync(ct);
		}

		public async Task DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
		{
			var set = ids?.ToHashSet() ?? [];
			if (set.Count == 0) return;

			await _db.CachedAccessPolicies
				.Where(x => set.Contains(x.Id))
				.ExecuteDeleteAsync(ct);
		}

		/// <inheritdoc />
		public async Task<CachedAccessPolicyEntity?> FindAsync(string caller, string target, CancellationToken ct = default)
		{
			var c = caller.ToLowerInvariant();
			var t = target.ToLowerInvariant();

			return await _db.CachedAccessPolicies
				.AsNoTracking()
				.FirstOrDefaultAsync(x =>
					x.CallerSystemName == c &&
					x.TargetSystemName == t, ct);
		}

		/// <inheritdoc />
		public async Task<Dictionary<(string caller, string target), CachedAccessPolicyEntity>> FindManyAsync(
			IEnumerable<(string caller, string target)> keys, CancellationToken ct = default)
		{
			var norm = keys
				.Select(k => (caller: k.caller.ToLowerInvariant(), target: k.target.ToLowerInvariant()))
				.Distinct()
				.ToList();

			if (norm.Count == 0) return new();

			var callers = norm.Select(k => k.caller).ToHashSet();
			var targets = norm.Select(k => k.target).ToHashSet();
			var candidates = await _db.CachedAccessPolicies
				.AsNoTracking()
				.Where(x => callers.Contains(x.CallerSystemName) && targets.Contains(x.TargetSystemName))
				.ToListAsync(ct);

			var set = norm.ToHashSet();
			return candidates
				.Where(x => set.Contains((x.CallerSystemName, x.TargetSystemName)))
				.ToDictionary(x => (x.CallerSystemName, x.TargetSystemName));
		}

		/// <inheritdoc />
		public async Task<CachedAccessPolicyEntity?> FindByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _db.CachedAccessPolicies
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == id, ct);
		}

		/// <inheritdoc />
		public async Task ReplaceAllAsync(IEnumerable<CachedAccessPolicyEntity> items, CancellationToken ct = default)
		{
			using var tx = await _db.Database.BeginTransactionAsync(ct);

			await _db.CachedAccessPolicies.ExecuteDeleteAsync(ct);
			var list = items.ToList();
			if (list.Count > 0)
				await _db.CachedAccessPolicies.AddRangeAsync(list, ct);

			await _db.SaveChangesAsync(ct);
			await tx.CommitAsync(ct);
		}

		/// <inheritdoc />
		public async Task ClearAsync(CancellationToken ct = default)
		{
			await _db.CachedAccessPolicies.ExecuteDeleteAsync(ct);
		}

		/// <inheritdoc />
		public async Task<List<CachedAccessPolicyEntity>> GetAllAsync(CancellationToken ct = default)
		{
			return await _db.CachedAccessPolicies.AsNoTracking().ToListAsync(ct);
		}
	}
}
