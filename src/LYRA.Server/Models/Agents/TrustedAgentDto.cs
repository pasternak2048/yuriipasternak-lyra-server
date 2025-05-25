namespace LYRA.Server.Models.Agents
{
    public class TrustedAgentDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public bool IsActive { get; set; }

        public string Mode { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
