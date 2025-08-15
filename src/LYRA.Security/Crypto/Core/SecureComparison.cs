using System.Text;

namespace LYRA.Security.Crypto.Core
{
	/// <summary>
	/// Provides constant-time string comparison to prevent timing attacks.
	/// Should be used when comparing sensitive values like signatures or tokens.
	///
	/// Design notes:
	/// - Comparison time depends only on length, not on content.
	/// - Works on UTF-8 strings, case-sensitive.
	/// </summary>
	public static class SecureComparison
	{
		/// <summary>
		/// Compares two strings in constant time (based on UTF-8 bytes).
		/// Returns false immediately if lengths mismatch.
		/// </summary>
		public static bool Equals(string a, string b)
		{
			if (a is null || b is null) return false;

			var aBytes = Encoding.UTF8.GetBytes(a);
			var bBytes = Encoding.UTF8.GetBytes(b);

			return Equals(aBytes, bBytes);
		}

		/// <summary>
		/// Compares two byte arrays in constant time.
		/// Returns false if lengths mismatch.
		/// </summary>
		public static bool Equals(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
		{
			if (a.Length != b.Length) return false;

			int diff = 0;

			for (int i = 0; i < a.Length; i++)
			{
				diff |= a[i] ^ b[i];
			}

			return diff == 0;
		}
	}
}
