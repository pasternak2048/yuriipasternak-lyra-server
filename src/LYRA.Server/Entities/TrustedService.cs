namespace LYRA.Server.Entities
{
    public class TrustedService
    {
        public Guid Id { get; set; }

        public string CompanyId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Secret { get; set; } = null!;

        public bool UseCompanySecret { get; set; } = false;
    }
}
