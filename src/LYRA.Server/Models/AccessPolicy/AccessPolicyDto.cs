using LYRA.Server.Enums;

namespace LYRA.Server.Models.AccessPolicy
{
    public class AccessPolicyDto
    {
        /// <summary>
        /// Unique identifier of the access policy.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the calling touchpoint.
        /// </summary>
        public Guid CallerId { get; set; }

        /// <summary>
        /// Cached system name of the caller.
        /// </summary>
        public string CallerSystemName { get; set; } = null!;

        /// <summary>
        /// ID of the receiving touchpoint.
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// Cached system name of the target.
        /// </summary>
        public string TargetSystemName { get; set; } = null!;

        /// <summary>
        /// Operation identifier (e.g. API path or message topic).
        /// </summary>
        public string Operation { get; set; } = null!;

        /// <summary>
        /// Type of context for the operation (http / event / cache / etc).
        /// </summary>
        public AccessContext Context { get; set; }

        /// <summary>
        /// Indicates whether this policy is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Creation timestamp (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
