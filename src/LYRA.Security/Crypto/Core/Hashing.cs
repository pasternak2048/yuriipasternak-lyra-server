using LYRA.Security.Enums;
using System.Security.Cryptography;
using System.Text;

namespace LYRA.Security.Crypto.Core
{
	/// <summary>
	/// Low-level hashing utilities.
	/// - Deterministic canonical hashing for payloads (SHA-512).
	/// - HMAC helpers for signing canonical "string-to-sign".
	/// - Byte/encoding helpers (Hex/Base64).
	///
	/// Design notes:
	/// - No transport assumptions; strings are always encoded as UTF-8.
	/// - The API exposes both byte[] and string-based overloads for convenience.
	/// - Callers choose output format: Base64 (default in docs) or Hex.
	/// </summary>
	public static class Hashing
	{
		private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

		// -------------------------
		// SHA-512 (payload hashing)
		// -------------------------

		/// <summary>
		/// Computes SHA-512 over the given UTF-8 string and returns Base64.
		/// Use this to hash request payloads before signing.
		/// </summary>
		public static string Sha512Base64(string input)
		{
			if (input is null) throw new ArgumentNullException(nameof(input));
			var bytes = Utf8.GetBytes(input);
			var hash = Sha512(bytes);
			return ToBase64(hash);
		}

		/// <summary>
		/// Computes SHA-512 over the given UTF-8 string and returns lowercase hex.
		/// </summary>
		public static string Sha512Hex(string input)
		{
			if (input is null) throw new ArgumentNullException(nameof(input));
			var bytes = Utf8.GetBytes(input);
			var hash = Sha512(bytes);
			return ToHex(hash);
		}

		/// <summary>
		/// Computes SHA-512 over the given bytes and returns the raw hash bytes (64 bytes).
		/// </summary>
		public static byte[] Sha512(ReadOnlySpan<byte> data)
		{
			using var sha = SHA512.Create();
			return sha.ComputeHash(data.ToArray());
		}

		// --------------------------------
		// HMAC (signing "string-to-sign")
		// --------------------------------

		/// <summary>
		/// Computes HMAC over the given UTF-8 string using the provided secret (UTF-8),
		/// with the specified signature algorithm. Returns Base64 by default (per docs).
		/// </summary>
		public static string HmacBase64(SignatureType type, string data, string secret)
		{
			if (data is null) throw new ArgumentNullException(nameof(data));
			if (secret is null) throw new ArgumentNullException(nameof(secret));

			var bytes = Utf8.GetBytes(data);
			var key = Utf8.GetBytes(secret);
			var mac = Hmac(type, bytes, key);
			return ToBase64(mac);
		}

		/// <summary>
		/// Computes HMAC over the given UTF-8 string and returns lowercase hex.
		/// </summary>
		public static string HmacHex(SignatureType type, string data, string secret)
		{
			if (data is null) throw new ArgumentNullException(nameof(data));
			if (secret is null) throw new ArgumentNullException(nameof(secret));

			var bytes = Utf8.GetBytes(data);
			var key = Utf8.GetBytes(secret);
			var mac = Hmac(type, bytes, key);
			return ToHex(mac);
		}

		/// <summary>
		/// Core HMAC routine (byte-based). Returns raw MAC bytes.
		/// </summary>
		public static byte[] Hmac(SignatureType type, ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
		{
			using HMAC hmac = type switch
			{
				SignatureType.HmacSha256 => new HMACSHA256(key.ToArray()),
				SignatureType.HmacSha512 => new HMACSHA512(key.ToArray()),
				_ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported signature type: {type}")
			};

			return hmac.ComputeHash(data.ToArray());
		}

		// ------------------------
		// Encoding helpers
		// ------------------------

		/// <summary>
		/// Converts bytes to lowercase hex (no separators).
		/// </summary>
		public static string ToHex(ReadOnlySpan<byte> bytes)
		{
			var c = new char[bytes.Length * 2];
			int ci = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = bytes[i];
				c[ci++] = GetHexNibble(b >> 4);
				c[ci++] = GetHexNibble(b & 0xF);
			}
			return new string(c);

			static char GetHexNibble(int v) => (char)(v < 10 ? '0' + v : 'a' + (v - 10));
		}

		/// <summary>
		/// Base64 encodes bytes.
		/// </summary>
		public static string ToBase64(ReadOnlySpan<byte> bytes) => Convert.ToBase64String(bytes.ToArray());

		/// <summary>
		/// Decodes Base64 into bytes (throws if invalid).
		/// </summary>
		public static byte[] FromBase64(string base64)
		{
			if (base64 is null) throw new ArgumentNullException(nameof(base64));
			return Convert.FromBase64String(base64);
		}

		// ------------------------
		// Convenience wrappers
		// ------------------------

		/// <summary>
		/// Convenience method used by callers to hash the payload consistently
		/// (UTF-8 → SHA-512 → Base64). Mirrors the format used in docs.
		/// </summary>
		public static string ComputePayloadHash(string payload) => Sha512Base64(payload);

		/// <summary>
		/// Convenience method for producing the final signature string (Base64) over a canonical string.
		/// </summary>
		public static string ComputeSignature(SignatureType type, string stringToSign, string secret) =>
			HmacBase64(type, stringToSign, secret);
	}
}
