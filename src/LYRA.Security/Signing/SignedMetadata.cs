using LYRA.Security.Enums;

namespace LYRA.Security.Signing
{
	/// <summary>
	/// Holds the cryptographic signature generated for a canonical string
	/// along with the algorithm type used to produce it.
	///
	/// This structure is transport-agnostic and can be attached to
	/// any request (HTTP headers, message payload, event envelope, etc.).
	/// </summary>
	public sealed class SignedMetadata
	{
		/// <summary>
		/// The signature algorithm used to produce this signature.
		/// Must match one of the supported <see cref="SignatureType"/>.
		/// </summary>
		public required SignatureType SignatureType { get; init; }

		/// <summary>
		/// The Base64-encoded signature over the canonical string.
		/// This value must match the result of applying the selected algorithm
		/// to the corresponding "string to sign".
		/// </summary>
		public required string Signature { get; init; }
	}
}
