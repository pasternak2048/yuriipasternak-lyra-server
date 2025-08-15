using LYRA.Security.Crypto.Abstractions;
using LYRA.Security.Crypto.Algorithms;
using LYRA.Security.Enums;

namespace LYRA.Security.Crypto.Core
{
	/// <summary>
	/// Provides a resolver for signature algorithm implementations based on SignatureType.
	/// Uses the strategy pattern to select the correct algorithm class.
	/// </summary>
	internal static class SignatureAlgorithmFactory
	{
		/// <summary>
		/// Resolves a signature algorithm strategy for the given signature type.
		/// </summary>
		/// <param name="type">The desired signature algorithm.</param>
		/// <returns>Concrete implementation of ISignatureAlgorithm.</returns>
		/// <exception cref="NotSupportedException">Thrown if the algorithm is not supported.</exception>
		public static ISignatureAlgorithm Resolve(SignatureType type) => type switch
		{
			SignatureType.HmacSha256 => new HmacSha256Algorithm(),
			SignatureType.HmacSha512 => new HmacSha512Algorithm(),

			//TODO:
			// SignatureType.RsaSha256 => new RsaSha256Algorithm(),
			// SignatureType.EcdsaP256 => new EcdsaP256Algorithm(),
			// SignatureType.Ed25519   => new Ed25519Algorithm(),

			_ => throw new NotSupportedException($"Unsupported signature type: {type}")
		};
	}
}
