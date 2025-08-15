using LYRA.Client.Abstractions;
using LYRA.Client.Models;
using LYRA.Security.Signing;

namespace LYRA.Client.Core
{
	/// <summary>
	/// Default implementation of the LYRA client.
	/// Responsible for signing outbound requests and verifying inbound ones.
	/// </summary>
	public sealed class LyraClient : ILyraClient
	{
		private readonly SigningService _signing;
		private readonly VerifyService _verify;

		public LyraClient(SigningService signing, VerifyService verify)
		{
			_signing = signing;
			_verify = verify;
		}

		/// <inheritdoc />
		public GenerateSignedMetadataResult GenerateSignedMetadata(GenericMetadata metadata, string? touchpointKey = null)
			=> _signing.Generate(metadata, touchpointKey);

		/// <inheritdoc />
		public Task<VerifyResponse> VerifyAsync(VerifyRequest request, CancellationToken ct = default)
			=> _verify.VerifyAsync(request, ct);
	}
}
