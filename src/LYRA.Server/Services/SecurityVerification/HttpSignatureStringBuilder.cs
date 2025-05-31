using LYRA.Server.Enums;
using LYRA.Server.Models.Verify;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Builds a canonical string to sign for requests in the <c>Http</c> access context.
    /// Used for verifying or generating signatures for HTTP-based inter-service communication.
    /// </summary>
    public class HttpSignatureStringBuilder : ISignatureStringBuilder
    {
        /// <summary>
        /// Gets the access context associated with this builder, which is <c>Http</c>.
        /// </summary>
        public AccessContext Context => AccessContext.Http;

        /// <summary>
        /// Builds the canonical string used for signature generation or verification for HTTP requests.
        /// </summary>
        /// <param name="request">The request containing relevant HTTP data.</param>
        /// <returns>A string representing the canonical form of the request for signature purposes.</returns>
        public string BuildStringToSign(VerifyRequest request)
        {
            return $"caller={request.Caller}&target={request.Target}&method={request.Method}&path={request.Path}&payloadHash={request.PayloadHash}&timestamp={request.Timestamp}";
        }
    }
}
