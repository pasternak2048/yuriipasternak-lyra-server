namespace LYRA.Server.Models.Company
{
    public class CompanyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string DisplayName { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
