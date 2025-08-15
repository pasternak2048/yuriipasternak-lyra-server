using LYRA.Client.Models;
using LYRA.Security.Signing;

namespace LYRA.Client.Abstractions
{
	/// <summary>
	/// LYRA client facade for outbound signing and inbound verification.
	/// </summary>
	public interface ILyraClient
	{
		/// <summary>
		/// Generates signed metadata for the provided canonical metadata.
		/// The metadata is signed using a registered local touchpoint.
		/// </summary>
		/// <param name="metadata">Canonical metadata (caller, target, action, etc.).</param>
		/// <param name="touchpointKey">Optional key to explicitly select a configured touchpoint.</param>
		/// <returns>The computed signature and canonical string.</returns>
		GenerateSignedMetadataResult GenerateSignedMetadata(GenericMetadata metadata, string? touchpointKey = null);

		/// <summary>
		/// Sends the request to LYRA.Server for verification.
		/// Used by the receiving system to validate incoming signed metadata.
		/// </summary>
		/// <param name="request">Verify request: metadata + signature.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>The verification result returned by the LYRA.Server.</returns>
		Task<VerifyResponse> VerifyAsync(VerifyRequest request, CancellationToken ct = default);
	}
}
