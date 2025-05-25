using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LYRA.Server.Entities
{
    public class AccessPolicyEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CallerAgentId { get; set; }

        [ForeignKey(nameof(CallerAgentId))]
        public TrustedAgentEntity CallerAgent { get; set; } = null!;

        [Required]
        public Guid TargetAgentId { get; set; }

        [ForeignKey(nameof(TargetAgentId))]
        public TrustedAgentEntity TargetAgent { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Method { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string PathPattern { get; set; } = null!;

        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
