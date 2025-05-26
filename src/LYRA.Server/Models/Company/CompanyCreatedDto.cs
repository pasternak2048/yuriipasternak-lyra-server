namespace LYRA.Server.Models.Company
{
    public class CompanyCreatedDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string DisplayName { get; set; } = default!;

        public string SecretPlaintext { get; set; } = default!;
    }
}
