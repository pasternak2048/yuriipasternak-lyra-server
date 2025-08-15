namespace LYRA.Security.Crypto.Abstractions
{
	/// <summary>
	/// Defines the contract for signature algorithms.
	/// Each implementation must produce and verify signatures
	/// based on a canonical string and shared secret.
	/// </summary>
	internal interface ISignatureAlgorithm
	{
		/// <summary>
		/// Computes a signature (Base64 encoded) for the given canonical string and secret.
		/// </summary>
		/// <param name="stringToSign">Canonical string (e.g., built from metadata).</param>
		/// <param name="secret">Secret key used for signing (in plain text).</param>
		/// <returns>Base64 encoded signature.</returns>
		string Sign(string stringToSign, string secret);

		/// <summary>
		/// Verifies whether the given signature is valid for the canonical string and secret.
		/// Must use constant-time comparison internally.
		/// </summary>
		/// <param name="stringToSign">Canonical string.</param>
		/// <param name="secret">Secret used to generate the signature.</param>
		/// <param name="expectedSignature">Provided signature (Base64 encoded).</param>
		/// <returns>True if the signature is valid; otherwise, false.</returns>
		bool Verify(string stringToSign, string secret, string expectedSignature);
	}
}
