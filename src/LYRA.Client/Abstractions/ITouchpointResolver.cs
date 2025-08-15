using LYRA.Client.Touchpoints;
using LYRA.Security.Signing;

namespace LYRA.Client.Abstractions
{
	/// <summary>
	/// Resolves the local signing configuration (secret + algorithm) for a given request.
	/// </summary>
	public interface ITouchpointResolver
	{
		/// <summary>
		/// Resolves a signing configuration for the specified metadata.
		/// If a touchpoint key is provided, it takes precedence.
		/// Otherwise, matching is attempted based on Caller and Target system names.
		/// </summary>
		/// <param name="metadata">Canonical metadata for which to resolve signing parameters.</param>
		/// <param name="touchpointKey">Optional alias/key for selecting a specific configured touchpoint.</param>
		/// <returns>Resolved secret and algorithm to use for signing.</returns>
		TouchpointBinding Resolve(GenericMetadata metadata, string? touchpointKey = null);
	}
}
