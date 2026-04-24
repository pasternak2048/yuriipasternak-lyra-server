using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYRA.Server.Entities
{
    /// <summary>
    /// Represents a single allowed route rule for an access policy.
    /// </summary>
    public class AccessPolicyRuleEntity
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Parent access policy identifier.
        /// </summary>
        [Required]
        public Guid AccessPolicyId { get; set; }

        /// <summary>
        /// Parent access policy.
        /// </summary>
        [ForeignKey(nameof(AccessPolicyId))]
        public AccessPolicyEntity AccessPolicy { get; set; } = null!;

        /// <summary>
        /// Allowed HTTP method.
        /// Example: GET, POST, PUT, DELETE.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string HttpMethod { get; set; } = null!;

        /// <summary>
        /// Allowed path pattern.
        /// Example: /api/orders or /api/orders/*
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string PathPattern { get; set; } = null!;
    }
}
