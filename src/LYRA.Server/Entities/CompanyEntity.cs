using LYRA.Server.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Entities
{
    /// <summary>
    /// Represents a tenant-level entity that owns Trusted Touchpoints.
    /// This is the core organizational unit in the system.
    /// </summary>
    public class CompanyEntity : IAuditableEntity
    {
        /// <summary>
        /// Primary key (GUID) that uniquely identifies the company.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Unique system identifier (slug/code) used for internal routing or addressing.
        /// Must be lowercase and unique.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Human-readable display name used in the UI.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// Shared secret used for authentication if no per-touchpoint secret is provided.
        /// Stored as a hashed string.
        /// </summary>
        [Required]
        public string Secret { get; set; } = null!;

        /// <summary>
        /// Indicates whether this company is currently active and can be used in access control.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Soft-delete flag: when true, this company is logically deleted and excluded from queries.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Collection of all Trusted Touchpoints registered under this company.
        /// </summary>
        public ICollection<TrustedTouchpointEntity> TrustedTouchpoints { get; set; } = new List<TrustedTouchpointEntity>();

        /// <summary>
        /// Audit: Date and time when the company was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Audit: Identifier of the user (GUID) who created this company.
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Audit: Last modification timestamp (UTC), if any changes occurred after creation.
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Audit: Identifier of the user (GUID) who last modified this company.
        /// </summary>
        public Guid? ModifiedBy { get; set; }
    }
}
