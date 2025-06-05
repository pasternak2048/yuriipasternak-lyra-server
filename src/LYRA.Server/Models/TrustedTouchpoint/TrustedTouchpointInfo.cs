namespace LYRA.Server.Models.TrustedTouchpoint
{
    /// <summary>
    /// Lightweight read-only DTO that holds essential information 
    /// about a trusted touchpoint and its associated company.
    /// Used for authorization and signature verification.
    /// </summary>
    public class TrustedTouchpointInfo
    {
        /// <summary>
        /// Unique identifier of the touchpoint.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Unique system name used in request validation.
        /// </summary>
        public string SystemName { get; set; } = default!;

        /// <summary>
        /// Encrypted secret key for HMAC signature validation (optional if using company secret).
        /// </summary>
        public string? Secret { get; set; }

        /// <summary>
        /// Indicates whether to use the company-level secret instead of the touchpoint's own.
        /// </summary>
        public bool UseCompanySecret { get; set; }

        /// <summary>
        /// Name of the company the touchpoint belongs to.
        /// </summary>
        public string CompanyName { get; set; } = default!;

        /// <summary>
        /// Encrypted secret of the company (used if UseCompanySecret is true).
        /// </summary>
        public string? CompanySecret { get; set; }

        /// <summary>
        /// Indicates whether the touchpoint is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates whether the owning company is active.
        /// </summary>
        public bool IsCompanyActive { get; set; }
    }
}
