using LYRA.Security.Enums;

namespace LYRA.Security.Internal
{
	/// <summary>
	/// Provides centralized argument validation to enforce defensive programming.
	/// Throws fast and explicitly when invalid inputs are passed to core logic.
	/// Internal use only — not part of the public API.
	/// </summary>
	internal static class Guard
	{
		/// <summary>
		/// Ensures that a reference is not null.
		/// </summary>
		public static void AgainstNull<T>(T? obj, string paramName) where T : class
		{
			if (obj is null)
				throw new ArgumentNullException(paramName);
		}

		/// <summary>
		/// Ensures that a string is not null, empty, or whitespace.
		/// </summary>
		public static void AgainstNullOrEmpty(string? value, string paramName)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException($"Parameter '{paramName}' cannot be null or empty.", paramName);
		}

		/// <summary>
		/// Ensures that a SignatureType enum value is defined.
		/// </summary>
		public static void AgainstInvalidSignatureType(SignatureType type)
		{
			if (!Enum.IsDefined(typeof(SignatureType), type))
				throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported signature type: {type}");
		}

		/// <summary>
		/// Ensures that the timestamp string is a valid positive UNIX timestamp.
		/// </summary>
		public static void AgainstInvalidTimestamp(string timestamp)
		{

			if (!long.TryParse(timestamp, out var unix) || unix <= 0 || unix < 1_000_000_000L)
				throw new ArgumentException("Invalid timestamp format (must be UNIX seconds).", nameof(timestamp));
		}

		/// <summary>
		/// Ensures that a byte array is not null or empty.
		/// </summary>
		public static void AgainstNullOrEmpty(ReadOnlySpan<byte> bytes, string paramName)
		{
			if (bytes == null || bytes.Length == 0)
				throw new ArgumentException($"Byte input '{paramName}' cannot be null or empty.");
		}
	}
}
