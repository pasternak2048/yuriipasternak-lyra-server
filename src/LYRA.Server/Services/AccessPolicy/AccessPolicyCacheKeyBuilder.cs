using LYRA.Server.Services.AccessPolicy.Interfaces;

namespace LYRA.Server.Services.AccessPolicy
{
	/// <summary>
	/// Default implementation of IAccessPolicyCacheKeyBuilder.
	/// Normalizes and formats keys consistently.
	/// </summary>
	public class AccessPolicyCacheKeyBuilder : IAccessPolicyCacheKeyBuilder
	{
		private const string PrefixByPair = "access:";
		private const string PrefixById = "access:id:";

		/// <inheritdoc />
		public string ForCallerTarget(string caller, string target)
		{
			var c = caller.ToLowerInvariant();
			var t = target.ToLowerInvariant();
			return $"{PrefixByPair}{c}:{t}";
		}

		/// <inheritdoc />
		public string ForId(Guid id)
		{
			return $"{PrefixById}{id:N}";
		}
	}
}
