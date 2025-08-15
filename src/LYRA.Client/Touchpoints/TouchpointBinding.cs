using LYRA.Security.Enums;

namespace LYRA.Client.Touchpoints
{
	/// <summary>
	/// Represents the resolved signing parameters for a touchpoint.
	/// Contains the secret and the algorithm used for signing.
	/// </summary>
	public sealed class TouchpointBinding
	{
		/// <summary>
		/// The secret used for signing (e.g., HMAC key).
		/// </summary>
		public required string Secret { get; init; }

		/// <summary>
		/// The algorithm used for signing.
		/// </summary>
		public required SignatureType SignatureType { get; init; }
	}
}
