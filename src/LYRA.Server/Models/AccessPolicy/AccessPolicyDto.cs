namespace LYRA.Server.Models.AccessPolicy
{
    /// <summary>
    /// Data transfer object representing an access policy.
    /// </summary>
    public class AccessPolicyDto
    {
        /// <summary>
        /// Unique identifier of the access policy.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the calling touchpoint (initiator).
        /// </summary>
        public Guid CallerId { get; set; }

        /// <summary>
        /// Cached system name of the calling touchpoint for performance.
        /// </summary>
        public string CallerSystemName { get; set; } = null!;

        /// <summary>
        /// ID of the target touchpoint (receiver).
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// Cached system name of the target touchpoint for performance.
        /// </summary>
        public string TargetSystemName { get; set; } = null!;

        /// <summary>
        /// Allowed route rules.
        /// </summary>
        public List<AccessRule> Rules { get; set; } = new();

        /// <summary>
        /// Indicates if the policy is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Timestamp when this policy was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
