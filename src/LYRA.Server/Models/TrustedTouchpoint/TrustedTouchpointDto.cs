using LYRA.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    public class TrustedTouchpointDto
    {
        /// <summary>
        /// Unique identifier of the touchpoint
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Machine-readable name (used for lookup)
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Display name (shown in UI)
        /// </summary>
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// ID of the company that owns the touchpoint
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Company name (for display only)
        /// </summary>
        public string CompanyName { get; set; } = null!;

        /// <summary>
        /// Indicates whether this touchpoint uses the company-wide secret
        /// </summary>
        public bool UseCompanySecret { get; set; }

        /// <summary>
        /// Whether this touchpoint is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Role of the touchpoint: CallerOnly, TargetOnly, or Both
        /// </summary>
        public TouchpointMode Mode { get; set; }

        /// <summary>
        /// Type of signature expected: HMAC, RSA, or None
        /// </summary>
        public SignatureType SignatureType { get; set; }

        /// <summary>
        /// Optional description for display or admin notes
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Date and time the touchpoint was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Optional expected source IP or CIDR
        /// </summary>
        public string? AllowedSourceIp { get; set; }
    }
}
