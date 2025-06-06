using LYRA.Security.Enums;
using LYRA.Server.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYRA.Server.Entities
{
    /// <summary>
    /// Represents a trusted integration point (touchpoint) for a company,
    /// capable of initiating or receiving signed requests.
    /// </summary>
    public class TrustedTouchpointEntity : IAuditableEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyEntity Company { get; set; } = null!;

        /// <summary>
        /// Globally unique system name in the format: slugified-touchpoint@slugified-company.
        /// Used for identification and signing.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Human-readable display name (e.g., for UI or logs).
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// Unique secret or public key used for request signature verification.
        /// </summary>
        [Required]
        public string Secret { get; set; } = null!;

        /// <summary>
        /// If true, the touchpoint will use the company's shared secret instead of its own.
        /// </summary>
        public bool UseCompanySecret { get; set; } = false;

        /// <summary>
        /// Indicates whether the touchpoint is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Indicates whether the touchpoint has been soft-deleted.
        /// Soft-deleted entries are typically excluded from active queries.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Defines the communication role of the touchpoint: CallerOnly, TargetOnly, or Both.
        /// </summary>
        public TouchpointMode Mode { get; set; } = TouchpointMode.Both;

        /// <summary>
        /// The signature type used for verifying requests (e.g., HMAC, RSA, None).
        /// </summary>
        public SignatureType SignatureType { get; set; } = SignatureType.HMAC;

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

        /// <summary>
        /// Outgoing access policies where this touchpoint is the caller.
        /// </summary>
        public ICollection<AccessPolicyEntity> OutgoingPolicies { get; set; } = new List<AccessPolicyEntity>();

        /// <summary>
        /// Incoming access policies where this touchpoint is the target.
        /// </summary>
        public ICollection<AccessPolicyEntity> IncomingPolicies { get; set; } = new List<AccessPolicyEntity>();

        /// <summary>
        /// True if this touchpoint is allowed to sign outgoing requests.
        /// </summary>
        [NotMapped]
        public bool ShouldSign => Mode is TouchpointMode.CallerOnly or TouchpointMode.Both;

        /// <summary>
        /// True if this touchpoint is allowed to receive and validate signed requests.
        /// </summary>
        [NotMapped]
        public bool ShouldAccept => Mode is TouchpointMode.TargetOnly or TouchpointMode.Both;

        /// <summary>
        /// Audit: The UTC timestamp when the touchpoint was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Audit: The ID of the user (if any) who created this touchpoint.
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Audit: The UTC timestamp when the touchpoint was last modified.
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Audit: The ID of the user (if any) who last modified this touchpoint.
        /// </summary>
        public Guid? ModifiedBy { get; set; }
    }
}
