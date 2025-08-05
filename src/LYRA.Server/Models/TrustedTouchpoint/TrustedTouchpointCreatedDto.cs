namespace LYRA.Server.Models.TrustedTouchpoint
{
    /// <summary>
    /// Represents the result of a trusted touchpoint creation operation, including sensitive info like the plaintext secret.
    /// </summary>
    public class TrustedTouchpointCreatedDto
    {
        /// <summary>
        /// Unique identifier of the trusted touchpoint.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// System-level unique name of the touchpoint (e.g., "gateway@corp").
        /// </summary>
        public string SystemName { get; set; } = default!;

        /// <summary>
        /// Human-readable display name for UI or logs.
        /// </summary>
        public string DisplayName { get; set; } = default!;

        /// <summary>
        /// Name of the company this touchpoint belongs to.
        /// </summary>
        public string CompanyName { get; set; } = default!;

        /// <summary>
        /// Signature algorithm used by this touchpoint (e.g., HMAC or RSA).
        /// </summary>
        public string SignatureType { get; set; } = default!;

        /// <summary>
        /// Indicates whether the touchpoint is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates whether the company-level secret is used instead of a unique one.
        /// </summary>
        public bool UseCompanySecret { get; set; }

        /// <summary>
        /// Creation timestamp (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The newly generated plaintext secret for this touchpoint (shown only once).
        /// </summary>
        public string SecretPlaintext { get; set; } = default!;
    }
}
