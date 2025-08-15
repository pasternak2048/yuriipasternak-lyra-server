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
		/// Action being performed (e.g., post, get, publish, invoke).
		/// Should be lower-case and concise.
		/// </summary>
		public required string Action { get; init; }

		/// <summary>
		/// The logical resource on which the action is being performed.
		/// Can be a URI path, topic name, or key (e.g., "/api/user", "events.user.created").
		/// </summary>
		public required string Resource { get; init; }

		/// <summary>
		/// Base64-encoded SHA-512 hash of the request body or payload.
		/// Use empty string if there is no payload.
		/// </summary>
		public required string PayloadHash { get; init; }

		/// <summary>
		/// Unix timestamp (seconds since epoch) when the signature was generated.
		/// Should be within an acceptable time window on the receiver side.
		/// </summary>
		public required string Timestamp { get; init; }
	}
}
