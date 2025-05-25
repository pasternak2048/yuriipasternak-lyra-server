using LYRA.Server.Enums;

namespace LYRA.Server.Models.Agents
{
    public class TrustedAgentDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public string Secret { get; set; } = null!;

        public bool UseCompanySecret { get; set; } = false;

        public bool IsActive { get; set; }

        public AgentMode Mode { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
