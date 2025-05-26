using LYRA.Server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYRA.Server.Entities
{
    public class AccessPolicyEntity
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the calling touchpoint (initiator)
        /// </summary>
        [Required]
        public Guid CallerId { get; set; }

        [ForeignKey(nameof(CallerId))]
        public TrustedTouchpointEntity Caller { get; set; } = null!;

        /// <summary>
        /// ID of the receiving touchpoint (target)
        /// </summary>
        [Required]
        public Guid TargetId { get; set; }

        [ForeignKey(nameof(TargetId))]
        public TrustedTouchpointEntity Target { get; set; } = null!;

        /// <summary>
        /// Operation identifier — path, topic, key or method
        /// Examples:
        ///   - "GET /api/orders/*" (http)
        ///   - "order.created" (event)
        ///   - "SET cache:user:*" (cache)
        ///   - "OrderService.CreateOrder" (grpc/internal)
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Operation { get; set; } = null!;

        /// <summary>
        /// Type of interaction: http / event / cache / grpc / internal / etc.
        /// </summary>
        [Required]
        public AccessContext Context { get; set; } = AccessContext.Http;

        /// <summary>
        /// Whether this policy is currently enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Timestamp when this policy was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
