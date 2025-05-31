using LYRA.Server.Models.Verify;

namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Service responsible for verifying the authenticity and integrity of incoming requests using digital signatures.
    /// </summary>
    public interface IVerifyService
    {
        /// <summary>
        /// Verifies the signature of an incoming request based on its context, method, path, and other parameters.
        /// </summary>
        /// <param name="request">The request containing data to verify and the expected signature.</param>
        /// <returns>A response indicating whether the signature is valid and, if not, the reason for failure.</returns>
        Task<VerifyResponse> Verify(VerifyRequest request);
    }
}
