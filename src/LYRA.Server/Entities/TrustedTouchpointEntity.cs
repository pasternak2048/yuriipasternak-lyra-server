using LYRA.Server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYRA.Server.Entities
{
    public class TrustedTouchpointEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyEntity Company { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Display name (UI-friendly, required)
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// Unique secret or public key used for signature verification
        /// </summary>
        [Required]
        public string Secret { get; set; } = null!;

        /// <summary>
        /// Whether this touchpoint uses the company-wide shared secret
        /// </summary>
        public bool UseCompanySecret { get; set; } = false;

        /// <summary>
        /// Indicates if this touchpoint is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Defines the role of this touchpoint: can call, accept, or both
        /// </summary>
        public TouchpointMode Mode { get; set; } = TouchpointMode.Both;

        /// <summary>
        /// Type of signature used for request verification (e.g., HMAC, RSA, None)
        /// </summary>
        public SignatureType SignatureType { get; set; } = SignatureType.HMAC;

        /// <summary>
        /// Optional description for admins (UI-friendly)
        /// </summary>
        [MaxLength(300)]
        public string? Description { get; set; }

        /// <summary>
        /// Optional source IP or CIDR expected for incoming requests
        /// </summary>
        [MaxLength(100)]
        public string? AllowedSourceIp { get; set; }

        public ICollection<AccessPolicyEntity> OutgoingPolicies { get; set; } = new List<AccessPolicyEntity>();
        public ICollection<AccessPolicyEntity> IncomingPolicies { get; set; } = new List<AccessPolicyEntity>();

        [NotMapped]
        public bool ShouldSign => Mode is TouchpointMode.CallerOnly or TouchpointMode.Both;

        [NotMapped]
        public bool ShouldAccept => Mode is TouchpointMode.TargetOnly or TouchpointMode.Both;
    }
}
