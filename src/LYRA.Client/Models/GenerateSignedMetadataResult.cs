using LYRA.Security.Signing;

namespace LYRA.Client.Models
{
	/// <summary>
	/// Represents the result of generating signed metadata.
	/// Includes both the signature and the canonical string used to compute it.
	/// </summary>
	public sealed class GenerateSignedMetadataResult
	{
		/// <summary>
		/// The signed metadata containing the signature and its algorithm.
		/// </summary>
		public required SignedMetadata Signed { get; init; }

		/// <summary>
		/// The canonical string that was signed.
		/// Useful for diagnostics or verification.
		/// </summary>
		public required string StringToSign { get; init; }
	}
}
