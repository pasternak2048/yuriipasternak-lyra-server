namespace LYRA.Server.Models.Company
{
    public class CompanyCreatedDto
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; } = default!;

        public string DisplayName { get; set; } = default!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string SecretPlaintext { get; set; } = default!;
    }
}
