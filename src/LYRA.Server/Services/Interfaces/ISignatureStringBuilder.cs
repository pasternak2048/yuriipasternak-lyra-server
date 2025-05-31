using LYRA.Server.Enums;
using LYRA.Server.Models.Verify;

namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Defines a strategy for building a canonical string to sign based on a specific access context.
    /// </summary>
    public interface ISignatureStringBuilder
    {
        /// <summary>
        /// Gets the access context this builder supports (e.g., Http, Event, Grpc).
        /// </summary>
        AccessContext Context { get; }

        /// <summary>
        /// Builds the canonical string that will be used to compute or verify a signature.
        /// </summary>
        /// <param name="request">The request data required to build the string.</param>
        /// <returns>The canonical string to be signed.</returns>
        string BuildStringToSign(VerifyRequest request);
    }
}
