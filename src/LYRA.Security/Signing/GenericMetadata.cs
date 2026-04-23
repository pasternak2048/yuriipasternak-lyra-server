namespace LYRA.Security.Signing
{
    /// <summary>
    /// Canonical metadata structure used to build the string to sign.
    /// All fields must be included in a fixed order to ensure signature consistency.
    /// This model is transport-agnostic and used across HTTP, Events, etc.
    /// </summary>
    public sealed class GenericMetadata
    {
        /// <summary>
        /// System name of the calling party (the signer).
        /// Must match a trusted touchpoint registered on the server.
        /// </summary>
        public required string CallerSystemName { get; init; }

        /// <summary>
        /// System name of the intended recipient (the target system).
        /// Used for access policy lookup and signature scoping.
        /// </summary>
        public required string TargetSystemName { get; init; }

        /// <summary>
        /// Method being performed (e.g., GET, POST, PUBLISH, INVOKE).
        /// For HTTP this should be the HTTP method.
        /// </summary>
        public required string Method { get; init; }

        /// <summary>
        /// Logical path or resource identifier.
        /// For HTTP this should be the request path, e.g. "/api/orders/create".
        /// </summary>
        public required string Path { get; init; }

        /// <summary>
        /// Base64-encoded SHA-512 hash of the request body or payload.
        /// If there is no payload, use the SHA-512 hash of an empty string.
        /// </summary>
        public required string BodyHash { get; init; }

        /// <summary>
        /// Unix timestamp (seconds since epoch) when the signature was generated.
        /// Should be within an acceptable time window on the receiver side.
        /// </summary>
        public required string Timestamp { get; init; }
    }
}
