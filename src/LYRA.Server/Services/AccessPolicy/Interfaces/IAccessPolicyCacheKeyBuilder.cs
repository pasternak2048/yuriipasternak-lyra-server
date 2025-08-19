namespace LYRA.Server.Services.AccessPolicy.Interfaces
{
	/// <summary>
	/// Provides consistent cache key generation for CachedAccessPolicyEntity.
	/// </summary>
	public interface IAccessPolicyCacheKeyBuilder
	{
		/// <summary>
		/// Builds a unique cache key based on caller and target system names.
		/// </summary>
		string ForCallerTarget(string caller, string target);

		/// <summary>
		/// Builds a unique cache key based on access policy ID.
		/// </summary>
		string ForId(Guid id);
	}
}
