using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    /// <summary>
    /// Request model used to create a new trusted touchpoint for a company.
    /// Touchpoints define integration endpoints that can initiate or receive signed requests.
    /// </summary>
    public class TrustedTouchpointCreateRequest
    {
        /// <summary>
        /// Identifier of the company that owns this touchpoint.
        /// </summary>
        [Required]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Human-readable name used in the UI and for generating the unique system name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// If true, the touchpoint will use the company's shared secret instead of a dedicated one.
        /// </summary>
        public bool UseCompanySecret { get; set; }

        /// <summary>
        /// Indicates whether the touchpoint is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Defines the role of this touchpoint: CallerOnly, TargetOnly, or Both.
        /// </summary>
        [Required]
        public string Mode { get; set; } = "Both";

        /// <summary>
        /// Specifies the type of signature expected: HMAC, RSA, or None.
        /// </summary>
        [Required]
        public string SignatureType { get; set; } = "HMAC";

        /// <summary>
        /// Optional description for administrative or documentation purposes.
        /// </summary>
        [MaxLength(300)]
        public string? Description { get; set; }

        /// <summary>
        /// Optional IP address or CIDR range allowed to send requests.
        /// </summary>
        [MaxLength(100)]
        public string? AllowedSourceIp { get; set; }
    }
}
