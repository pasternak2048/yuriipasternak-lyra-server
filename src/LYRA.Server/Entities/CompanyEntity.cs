using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Entities
{
    public class CompanyEntity
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Unique company name (machine-readable, e.g. slug or code)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Optional display name (UI-friendly)
        /// </summary>
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Company-wide shared secret (can be used by touchpoints if allowed)
        /// </summary>
        [Required]
        public string Secret { get; set; } = null!;

        /// <summary>
        /// Indicates whether the company is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the company was registered
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Trusted touchpoints that belong to this company
        /// </summary>
        public ICollection<TrustedTouchpointEntity> TrustedTouchpoints { get; set; } = new List<TrustedTouchpointEntity>();
    }
}
