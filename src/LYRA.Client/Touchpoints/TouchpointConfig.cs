using LYRA.Security.Enums;

namespace LYRA.Client.Touchpoints
{
    /// <summary>
    /// Configuration for a local signing identity.
    /// Used by the sending system to sign outbound requests as a caller.
    /// </summary>
    public sealed class TouchpointConfig
    {
        /// <summary>
        /// Optional alias/key to identify this signing identity explicitly.
        /// Useful when multiple caller identities exist in the same application.
        /// </summary>
        public string? Key { get; init; }

        /// <summary>
        /// The name of the system that is sending the request.
        /// Must match what LYRA.Server expects as the caller identity.
        /// </summary>
        public required string CallerSystemName { get; init; }

        /// <summary>
        /// The secret used for signing. This should be kept safe.
        /// Typically an HMAC key shared with LYRA.Server.
        /// </summary>
        public required string Secret { get; init; }

        /// <summary>
        /// The algorithm to use for signing this caller identity.
        /// </summary>
        public required SignatureType SignatureType { get; init; }
    }
}
