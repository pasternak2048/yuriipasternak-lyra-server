using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Entities
{
    /// <summary>
    /// Represents a flattened, cache-friendly version of an access policy between two systems.
    /// </summary>
    public class CachedAccessPolicyEntity
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Denormalized system name of the caller touchpoint (e.g., gateway@company).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CallerSystemName { get; set; } = null!;

        /// <summary>
        /// Denormalized system name of the target touchpoint (e.g., billing@company).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TargetSystemName { get; set; } = null!;

        /// <summary>
        /// Operation identifier (e.g., GET /api/orders/*, order.created).
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Operation { get; set; } = null!;

        /// <summary>
        /// Context of the request (Http, Event, Cache, etc.). Stored as string.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Context { get; set; } = null!;

        /// <summary>
        /// Secret used for verifying the signature of the caller (resolved from Caller or Company).
        /// </summary>
        [Required]
        public string CallerSecret { get; set; } = null!;

        /// <summary>
        /// Signature type used (e.g., HMAC, RSA).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string SignatureType { get; set; } = null!;

        /// <summary>
        /// Whether this policy is currently enabled (cached only if true).
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Optional IP or CIDR range allowed for incoming requests.
        /// </summary>
        [MaxLength(100)]
        public string? AllowedSourceIp { get; set; }

        /// <summary>
        /// Company identifier (slug) for the caller.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CallerCompanySystemName { get; set; } = null!;

        /// <summary>
        /// Company identifier (slug) for the target.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TargetCompanySystemName { get; set; } = null!;

        /// <summary>
        /// When the cached policy was generated.
        /// </summary>
        public DateTime CachedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
