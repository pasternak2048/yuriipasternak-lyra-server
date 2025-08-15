namespace LYRA.Client.Core
{
	/// <summary>
	/// Provides consistent UTC timestamp values for signature generation.
	/// </summary>
	public static class TimeProvider
	{
		/// <summary>
		/// Returns the current UTC time as a UNIX timestamp (seconds since epoch).
		/// </summary>
		public static long UnixSeconds()
			=> DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		/// <summary>
		/// Returns the current UTC time as a string representation of UNIX timestamp.
		/// This is used directly in metadata.
		/// </summary>
		public static string UnixSecondsString()
			=> UnixSeconds().ToString();
	}
}
