using LYRA.Client.Abstractions;
using LYRA.Client.Models;
using LYRA.Security.Crypto.Core;
using LYRA.Security.Signing;

namespace LYRA.Client.Core
{
	/// <summary>
	/// Handles signing of metadata using registered touchpoints and selected algorithm.
	/// </summary>
	public sealed class SigningService
	{
		private readonly ITouchpointResolver _resolver;

		public SigningService(ITouchpointResolver resolver)
		{
			_resolver = resolver;
		}

		/// <summary>
		/// Generates signed metadata based on the provided canonical metadata.
		/// Automatically resolves the appropriate secret and algorithm.
		/// </summary>
		/// <param name="metadata">Canonical metadata to sign.</param>
		/// <param name="touchpointKey">Optional key for a specific touchpoint.</param>
		/// <returns>The resulting signature and the canonical string used for signing.</returns>
		public GenerateSignedMetadataResult Generate(GenericMetadata metadata, string? touchpointKey = null)
		{
			var binding = _resolver.Resolve(metadata, touchpointKey);
			var stringToSign = SignatureStringBuilder.BuildStringToSign(metadata);

			var signature = Signer.Sign(stringToSign, binding.Secret, binding.SignatureType);

			return new GenerateSignedMetadataResult
			{
				StringToSign = stringToSign,
				Signed = new SignedMetadata
				{
					Signature = signature,
					SignatureType = binding.SignatureType
				}
			};
		}
	}
}
