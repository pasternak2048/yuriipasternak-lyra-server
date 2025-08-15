using LYRA.Security.Enums;

namespace LYRA.Client.Touchpoints
{
	/// <summary>
	/// Configuration for a local signing touchpoint.
	/// Used by the sending system to sign requests.
	/// </summary>
	public sealed class TouchpointConfig
	{
		/// <summary>
		/// Optional alias/key to identify this touchpoint explicitly.
		/// Useful when multiple touchpoints exist in the same application.
		/// </summary>
		public string? Key { get; init; }

		/// <summary>
		/// The name of the system that is sending the request.
		/// Must match what LYRA.Server expects as the "caller".
		/// </summary>
		public required string CallerSystemName { get; init; }

		/// <summary>
		/// The name of the system that is expected to receive the request.
		/// Must match what LYRA.Server expects as the "target".
		/// </summary>
		public required string TargetSystemName { get; init; }

		/// <summary>
		/// The secret used for signing. This should be kept safe.
		/// Typically an HMAC key shared with LYRA.Server.
		/// </summary>
		public required string Secret { get; init; }

		/// <summary>
		/// The algorithm to use for signing this touchpoint.
		/// </summary>
		public required SignatureType SignatureType { get; init; }
	}
}
