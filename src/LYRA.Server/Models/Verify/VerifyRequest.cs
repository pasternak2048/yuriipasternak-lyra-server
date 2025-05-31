using LYRA.Server.Enums;

namespace LYRA.Server.Models.Verify
{
    /// <summary>
    /// Represents a request to verify a signed interaction between two trusted touchpoints.
    /// </summary>
    public class VerifyRequest
    {
        /// <summary>
        /// System name of the initiating touchpoint (e.g., "gateway@bcorp").
        /// </summary>
        public string Caller { get; set; } = null!;

        /// <summary>
        /// System name of the receiving touchpoint (e.g., "billing@acorp").
        /// </summary>
        public string Target { get; set; } = null!;

        /// <summary>
        /// HTTP method or action type (e.g., "POST", "GET").
        /// </summary>
        public string Method { get; set; } = null!;

        /// <summary>
        /// Request path or resource identifier (e.g., "/subscribe").
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// Base64-encoded hash of the request payload.
        /// </summary>
        public string PayloadHash { get; set; } = null!;

        /// <summary>
        /// UTC timestamp at the time the request was signed.
        /// </summary>
        public string Timestamp { get; set; } = null!;

        /// <summary>
        /// Type of interaction being verified (e.g., Http, Event, Grpc).
        /// </summary>
        public AccessContext Context { get; set; }

        /// <summary>
        /// Base64-encoded signature of the full verification string.
        /// </summary>
        public string Signature { get; set; } = null!;
    }
}