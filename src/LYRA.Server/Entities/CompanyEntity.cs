using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Entities
{
    public class CompanyEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [Required]
        public string Secret { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TrustedAgentEntity> Agents { get; set; } = new List<TrustedAgentEntity>();
    }
}
