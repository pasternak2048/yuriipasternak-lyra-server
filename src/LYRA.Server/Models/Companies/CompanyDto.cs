namespace LYRA.Server.Models.Companies
{
    public class CompanyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? DisplayName { get; set; }

        public string Secret { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
