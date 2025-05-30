using LYRA.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.AccessPolicy
{
    public class AccessPolicyCreateRequest
    {
        /// <summary>
        /// Unique system name of the calling touchpoint (alternative to CallerId)
        /// </summary>
        [MaxLength(100)]
        public string? CallerSystemName { get; set; }

        /// <summary>
        /// ID of the calling touchpoint (alternative to CallerSystemName)
        /// </summary>
        public Guid? CallerId { get; set; }

        /// <summary>
        /// Unique system name of the target touchpoint (alternative to TargetId)
        /// </summary>
        [MaxLength(100)]
        public string? TargetSystemName { get; set; }

        /// <summary>
        /// ID of the target touchpoint (alternative to TargetSystemName)
        /// </summary>
        public Guid? TargetId { get; set; }

        /// <summary>
        /// Operation identifier (path, topic, or method)
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Operation { get; set; } = null!;

        /// <summary>
        /// Context of the access policy (http, event, grpc, etc.)
        /// </summary>
        [Required]
        public AccessContext Context { get; set; }

        /// <summary>
        /// Whether this policy is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }

}
