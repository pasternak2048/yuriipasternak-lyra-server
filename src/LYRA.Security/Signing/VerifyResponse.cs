namespace LYRA.Security.Signing
{
	/// <summary>
	/// Represents the result of verifying a signature via the LYRA server.
	/// Indicates whether the signature is valid and, if not, provides a reason.
	///
	/// This model is used as the response body of the /api/verify endpoint
	/// and can also be returned by local verification logic.
	/// </summary>
	public sealed class VerifyResponse
	{
		/// <summary>
		/// Indicates whether the signature is valid according to LYRA rules:
		/// - Matching signature
		/// - Allowed by access policy
		/// - Timestamp is within acceptable range
		/// </summary>
		public required bool Success { get; init; }

		/// <summary>
		/// Optional reason for failure (when Success == false).
		/// Examples: "BadSignature", "PolicyDenied", "ReplayDetected", "Expired", etc.
		/// </summary>
		public string? Reason { get; init; }

		/// <summary>
		/// Optional extra details for debugging or logging.
		/// May include touchpoint names, timestamp offset, etc.
		/// </summary>
		public string? Details { get; init; }
	}
}
