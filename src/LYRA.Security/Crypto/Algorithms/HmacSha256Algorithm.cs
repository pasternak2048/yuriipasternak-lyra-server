using LYRA.Security.Crypto.Abstractions;
using LYRA.Security.Crypto.Core;
using LYRA.Security.Enums;

namespace LYRA.Security.Crypto.Algorithms
{
	/// <summary>
	/// Provides HMAC-SHA-256 signature generation and verification.
	/// Produces Base64-encoded signatures using a shared secret.
	/// </summary>
	internal sealed class HmacSha256Algorithm : ISignatureAlgorithm
	{
		/// <summary>
		/// Computes a Base64-encoded HMAC-SHA256 signature for the canonical string.
		/// </summary>
		/// <param name="stringToSign">Canonical input string (e.g., built from metadata).</param>
		/// <param name="secret">Plaintext secret used for signing.</param>
		public string Sign(string stringToSign, string secret)
		{
			return Hashing.HmacBase64(SignatureType.HmacSha256, stringToSign, secret);
		}

		/// <summary>
		/// Verifies a provided Base64 signature against a canonical string and secret.
		/// Uses constant-time comparison to prevent timing attacks.
		/// </summary>
		public bool Verify(string stringToSign, string secret, string expectedSignature)
		{
			var actual = Sign(stringToSign, secret);
			return SecureComparison.Equals(actual, expectedSignature);
		}
	}
}
