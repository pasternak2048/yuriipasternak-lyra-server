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
		/// Tries to get cached policy from RAM; on miss fetches from persistent storage and populates RAM.
		/// </summary>
		Task<CachedAccessPolicyEntity?> GetAsync(string caller, string target, CancellationToken ct = default);

		/// <summary>
		/// Force refresh of RAM entry by reloading from persistent storage (no-op if not found there).
		/// </summary>
		Task RefreshAsync(string caller, string target, CancellationToken ct = default);

		/// <summary>
		/// Invalidate RAM entry (both normalized and, якщо треба, старі ключі).
		/// </summary>
		void Invalidate(string caller, string target);

		/// <summary>
		/// Batch get with fallback; returns dictionary only for found items.
		/// </summary>
		Task<Dictionary<(string caller, string target), CachedAccessPolicyEntity>> GetManyAsync(
			IEnumerable<(string caller, string target)> keys, CancellationToken ct = default);

		/// <summary>
		/// Batch refresh of RAM entries from persistent storage.
		/// </summary>
		Task RefreshManyAsync(IEnumerable<(string caller, string target)> keys, CancellationToken ct = default);

		/// <summary>
		/// Batch invalidate RAM entries.
		/// </summary>
		void InvalidateMany(IEnumerable<(string caller, string target)> keys);

		/// <summary>
		/// Preload RAM from persistent cache (optional subset via predicate).
		/// </summary>
		Task WarmupAsync(Func<CachedAccessPolicyEntity, bool>? filter = null, CancellationToken ct = default);

		/// <summary>
		/// Clear entire RAM cache namespace for policies.
		/// </summary>
		void Clear();
	}
}
