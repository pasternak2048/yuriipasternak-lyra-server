using LYRA.Security.Enums;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    /// <summary>
    /// Data transfer object representing a trusted integration touchpoint.
    /// Used to display detailed information in admin panels or APIs.
    /// </summary>
    public class TrustedTouchpointDto
    {
        /// <summary>
        /// Unique identifier of the touchpoint.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// System name used for request validation and identification (format: touchpoint@company).
        /// </summary>
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Human-readable display name for UI and management purposes.
        /// </summary>
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// Identifier of the company that owns this touchpoint.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Display name of the company (for read-only or UI display).
        /// </summary>
        public string CompanyName { get; set; } = null!;

        /// <summary>
        /// Indicates whether this touchpoint inherits the company's secret.
        /// </summary>
        public bool UseCompanySecret { get; set; }

        /// <summary>
        /// Indicates whether this touchpoint is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Type of digital signature expected: HMAC, RSA, or none.
        /// </summary>
        public SignatureType SignatureType { get; set; }

        /// <summary>
        /// Optional description for administrative purposes.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// UTC timestamp of when the touchpoint was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Optional IP address or CIDR mask allowed to send requests.
        /// </summary>
        public string? AllowedSourceIp { get; set; }
    }
}
