using LYRA.Security.Crypto.Core;

namespace LYRA.Client.Core
{
	/// <summary>
	/// Provides a consistent way to compute a hash for payloads.
	/// Uses SHA-512 and returns the hash as a Base64 string.
	/// </summary>
	public static class PayloadHasher
	{
		/// <summary>
		/// Computes a Base64-encoded SHA-512 hash of the payload.
		/// If the payload is null or empty, returns an empty string.
		/// </summary>
		/// <param name="payload">The raw payload string to hash (e.g., JSON).</param>
		/// <returns>Base64-encoded SHA-512 hash.</returns>
		public static string Sha512Base64(string payload)
			=> string.IsNullOrWhiteSpace(payload)
				? string.Empty
				: Hashing.Sha512Base64(payload);
	}
}
