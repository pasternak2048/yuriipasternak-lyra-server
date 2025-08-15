using LYRA.Server.Entities;

namespace LYRA.Server.Services.AccessPolicy.Interfaces
{
	/// <summary>
	/// Service for managing cached access policies in the LyraCachedDbContext.
	/// Used to optimize runtime verification by storing denormalized access data
	/// for fast, memory-friendly lookups.
	/// </summary>
	public interface ICachedAccessPolicyService
	{
		/// <summary>
		/// Inserts or updates a single cached access policy in the database.
		/// If a policy for the given caller-target pair exists, it will be updated.
		/// </summary>
		Task UpsertAsync(CachedAccessPolicyEntity item, CancellationToken ct = default);

		/// <summary>
		/// Deletes a cached policy by its unique identifier (Id).
		/// </summary>
		Task DeleteByIdAsync(Guid id, CancellationToken ct = default);

		/// <summary>
		/// Finds a cached policy based on caller and target system names.
		/// Returns null if not found. Keys are case-insensitive.
		/// </summary>
		Task<CachedAccessPolicyEntity?> FindAsync(string caller, string target, CancellationToken ct = default);

		/// <summary>
		/// Finds a cached policy by its unique identifier (Id).
		/// Returns null if not found.
		/// </summary>
		Task<CachedAccessPolicyEntity?> FindByIdAsync(Guid id, CancellationToken ct = default);

		/// <summary>
		/// Inserts or updates a batch of cached policies in a single transaction.
		/// Ideal for syncing large updates to the cache layer.
		/// </summary>
		Task UpsertManyAsync(IEnumerable<CachedAccessPolicyEntity> items, CancellationToken ct = default);

		/// <summary>
		/// Deletes multiple cached policies based on their AccessPolicyIds.
		/// All deletions happen in a single transaction.
		/// </summary>
		Task DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken ct = default);

		/// <summary>
		/// Finds multiple cached policies by (caller, target) pairs.
		/// Keys are case-insensitive. Returns a dictionary of matched pairs.
		/// </summary>
		Task<Dictionary<(string caller, string target), CachedAccessPolicyEntity>> FindManyAsync(
			IEnumerable<(string caller, string target)> keys, CancellationToken ct = default);

		/// <summary>
		/// Completely replaces the current cache with the provided list of policies.
		/// Clears all existing entries before inserting new ones.
		/// </summary>
		Task ReplaceAllAsync(IEnumerable<CachedAccessPolicyEntity> items, CancellationToken ct = default);

		/// <summary>
		/// Deletes all cached access policies from the storage.
		/// Use with caution — this clears the entire cache.
		/// </summary>
		Task ClearAsync(CancellationToken ct = default);

		/// <summary>
		/// Returns all cached access policies in the system.
		/// Avoid using in high-load paths due to potential memory usage.
		/// </summary>
		Task<List<CachedAccessPolicyEntity>> GetAllAsync(CancellationToken ct = default);
	}
}
