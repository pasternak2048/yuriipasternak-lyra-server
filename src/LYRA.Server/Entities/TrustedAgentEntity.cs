using LYRA.Server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYRA.Server.Entities
{
    public class TrustedAgentEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyEntity Company { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public string Secret { get; set; } = null!;

        public bool UseCompanySecret { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AgentMode Mode { get; set; } = AgentMode.Both;

        public bool ShouldSign => Mode is AgentMode.CallerOnly or AgentMode.Both;
        public bool ShouldAccept => Mode is AgentMode.TargetOnly or AgentMode.Both;

        public ICollection<AccessPolicyEntity> OutgoingPolicies { get; set; } = new List<AccessPolicyEntity>();
        public ICollection<AccessPolicyEntity> IncomingPolicies { get; set; } = new List<AccessPolicyEntity>();
    }
}
