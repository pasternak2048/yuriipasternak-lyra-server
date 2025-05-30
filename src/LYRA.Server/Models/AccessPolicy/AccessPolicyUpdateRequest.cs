using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.AccessPolicy
{
    public class AccessPolicyUpdateRequest : AccessPolicyCreateRequest
    {
        /// <summary>
        /// Unique identifier of the policy to be updated.
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}
