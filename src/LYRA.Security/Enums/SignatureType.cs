namespace LYRA.Security.Enums
{
	/// <summary>
	/// Represents supported signature algorithms used for request signing.
	/// Determines how the canonical string is signed using a secret key.
	/// </summary>
	public enum SignatureType
	{
		/// <summary>
		/// HMAC using SHA-256.
		/// Fast and widely supported. 32-byte signature.
		/// Suitable for most use cases.
		/// </summary>
		HmacSha256 = 0,

		/// <summary>
		/// HMAC using SHA-512.
		/// Longer signature (64 bytes) and stronger cryptographic guarantees.
		/// Default in LYRA.
		/// </summary>
		HmacSha512 = 1,

		// TODO:

		/// <summary>
		/// RSA with SHA-256 (RSASSA-PKCS1-v1_5).
		/// Asymmetric signing; requires private/public key pair.
		/// </summary>
		// RsaSha256 = 2,

		/// <summary>
		/// ECDSA with P-256 and SHA-256.
		/// Compact asymmetric signature for constrained devices.
		/// </summary>
		// EcdsaP256 = 3,

		/// <summary>
		/// Ed25519 digital signature (RFC 8032).
		/// Very fast and secure asymmetric signature.
		/// </summary>
		// Ed25519 = 4
	}
}
