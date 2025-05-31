using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.AccessPolicy
{
    /// <summary>
    /// Request model for updating an existing access policy.
    /// Inherits from AccessPolicyCreateRequest and adds the policy's unique identifier.
    /// </summary>
    public class AccessPolicyUpdateRequest : AccessPolicyCreateRequest
    {
        /// <summary>
        /// Unique identifier of the policy to be updated.
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}
