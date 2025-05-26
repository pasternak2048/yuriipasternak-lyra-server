using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    public class TrustedTouchpointCreateRequest
    {
        /// <summary>
        /// ID of the company that owns the touchpoint
        /// </summary>
        [Required]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Human-readable name of the touchpoint
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Secret or public key for the touchpoint
        /// </summary>
        [Required]
        public string Secret { get; set; } = null!;

        /// <summary>
        /// Whether to use the company-wide secret instead of individual one
        /// </summary>
        public bool UseCompanySecret { get; set; }

        /// <summary>
        /// Whether this touchpoint is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Touchpoint role: CallerOnly, TargetOnly, Both
        /// </summary>
        [Required]
        public string Mode { get; set; } = "Both";

        /// <summary>
        /// Signature type: HMAC, RSA, None
        /// </summary>
        [Required]
        public string SignatureType { get; set; } = "HMAC";

        /// <summary>
        /// Optional description for display or admin notes
        /// </summary>
        [MaxLength(300)]
        public string? Description { get; set; }

        /// <summary>
        /// Optional expected source IP or CIDR
        /// </summary>
        [MaxLength(100)]
        public string? AllowedSourceIp { get; set; }
    }
}
