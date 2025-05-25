using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Agents
{
    public class TrustedAgentCreateRequest
    {
        public Guid CompanyId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public string Secret { get; set; } = null!;

        public bool UseCompanySecret { get; set; }

        public bool IsActive { get; set; } = true;

        public string Mode { get; set; } = "Both";
    }
}
