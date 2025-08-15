using LYRA.Client.Touchpoints;

namespace LYRA.Client.Configuration
{
	/// <summary>
	/// Options used to configure local signing touchpoints
	/// for outbound requests from this service.
	/// </summary>
	public sealed class LyraSigningOptions
	{
		/// <summary>
		/// Collection of local touchpoints used for signing requests.
		/// Each touchpoint defines caller, target, secret and algorithm.
		/// </summary>
		public List<TouchpointConfig> Touchpoints { get; } = new();
	}
}
