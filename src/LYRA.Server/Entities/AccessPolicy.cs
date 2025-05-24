namespace LYRA.Server.Entities
{
    public class AccessPolicy
    {
        public Guid Id { get; set; }

        public string CompanyId { get; set; } = null!;

        public string Caller { get; set; } = null!;

        public string Target { get; set; } = null!;

        public string Method { get; set; } = null!;

        public string PathPattern { get; set; } = null!;
    }
}
