using LYRA.Security.Enums;
using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.AccessPolicy
{
    /// <summary>
    /// Request model for creating a new access policy.
    /// </summary>
    public class AccessPolicyCreateRequest
    {
        /// <summary>
        /// Unique system name of the calling touchpoint (alternative to CallerId).
        /// </summary>
        [MaxLength(100)]
        public string? CallerSystemName { get; set; }

        /// <summary>
        /// ID of the calling touchpoint (alternative to CallerSystemName).
        /// </summary>
        public Guid? CallerId { get; set; }

        /// <summary>
        /// Unique system name of the target touchpoint (alternative to TargetId).
        /// </summary>
        [MaxLength(100)]
        public string? TargetSystemName { get; set; }

        /// <summary>
        /// ID of the target touchpoint (alternative to TargetSystemName).
        /// </summary>
        public Guid? TargetId { get; set; }

        /// <summary>
        /// Operation identifier, such as an HTTP path, event topic, or method name.
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public List<string> Operations { get; set; } = new();

        /// <summary>
        /// Indicates whether this access policy is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}
