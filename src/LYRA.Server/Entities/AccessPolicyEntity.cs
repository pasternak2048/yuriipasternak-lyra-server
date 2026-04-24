using LYRA.Server.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYRA.Server.Entities
{
    /// <summary>
    /// Defines a permission relationship between two trusted touchpoints.
    /// Route rules are stored separately in <see cref="AccessPolicyRuleEntity"/>.
    /// </summary>
    public class AccessPolicyEntity : IAuditableEntity
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the calling touchpoint (initiator).
        /// </summary>
        [Required]
        public Guid CallerId { get; set; }

        /// <summary>
        /// Navigation property for the caller touchpoint.
        /// </summary>
        [ForeignKey(nameof(CallerId))]
        public TrustedTouchpointEntity Caller { get; set; } = null!;

        /// <summary>
        /// ID of the receiving touchpoint (target).
        /// </summary>
        [Required]
        public Guid TargetId { get; set; }

        /// <summary>
        /// Navigation property for the target touchpoint.
        /// </summary>
        [ForeignKey(nameof(TargetId))]
        public TrustedTouchpointEntity Target { get; set; } = null!;

        /// <summary>
        /// Cached system name of the caller (denormalized for performance).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CallerSystemName { get; set; } = null!;

        /// <summary>
        /// Cached system name of the target (denormalized for performance).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TargetSystemName { get; set; } = null!;

        /// <summary>
        /// Route rules allowed for this caller -> target policy.
        /// </summary>
        public ICollection<AccessPolicyRuleEntity> Rules { get; set; } = new List<AccessPolicyRuleEntity>();

        /// <summary>
        /// Whether this policy is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Audit: Date and time when the policy was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Audit: Identifier of the user (GUID) who created this policy.
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Audit: Last modification timestamp (UTC), if any changes occurred after creation.
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Audit: Identifier of the user (GUID) who last modified this policy.
        /// </summary>
        public Guid? ModifiedBy { get; set; }
    }
}
