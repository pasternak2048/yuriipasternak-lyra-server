using LYRA.Security.Enums;

namespace LYRA.Security.Crypto.Core
{
	/// <summary>
	/// Entry point for signature generation and verification.
	/// Uses the selected algorithm based on SignatureType.
	/// </summary>
	public static class Signer
	{
		/// <summary>
		/// Signs with the default LYRA algorithm (HMAC-SHA512).
		/// </summary>
		public static string Sign(string stringToSign, string secret)
			=> Sign(stringToSign, secret, SignatureType.HmacSha512);

		/// <summary>
		/// Verifies a signature using the default LYRA algorithm (HMAC-SHA512).
		/// </summary>
		public static bool Verify(string s, string secret, string expectedSignature)
			=> Verify(s, secret, expectedSignature, SignatureType.HmacSha512);

		/// <summary>
		/// Signs a canonical string using the provided secret and algorithm type.
		/// Returns a Base64-encoded signature.
		/// </summary>
		/// <param name="stringToSign">Canonical string generated from metadata.</param>
		/// <param name="secret">Secret key used for signing.</param>
		/// <param name="type">Signature algorithm.</param>
		public static string Sign(string stringToSign, string secret, SignatureType type)
		{
			var algorithm = SignatureAlgorithmFactory.Resolve(type);
			return algorithm.Sign(stringToSign, secret);
		}

		/// <summary>
		/// Verifies a provided signature against a canonical string and secret using the specified algorithm.
		/// Uses time-constant comparison to prevent timing attacks.
		/// </summary>
		/// <param name="stringToSign">Canonical string generated from metadata.</param>
		/// <param name="secret">Secret key used for signing.</param>
		/// <param name="expectedSignature">Signature to verify (Base64 encoded).</param>
		/// <param name="type">Signature algorithm.</param>
		/// <returns>True if signature is valid; otherwise false.</returns>
		public static bool Verify(string stringToSign, string secret, string expectedSignature, SignatureType type)
		{
			var algorithm = SignatureAlgorithmFactory.Resolve(type);
			return algorithm.Verify(stringToSign, secret, expectedSignature);
		}
	}
}
