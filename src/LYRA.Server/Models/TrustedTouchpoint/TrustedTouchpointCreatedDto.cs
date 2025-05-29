namespace LYRA.Server.Models.TrustedTouchpoint
{
    public class TrustedTouchpointCreatedDto
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; } = default!;

        public string DisplayName { get; set; } = default!;

        public string CompanyName { get; set; } = default!;

        public string Mode { get; set; } = default!;

        public string SignatureType { get; set; } = default!;

        public bool IsActive { get; set; }

        public bool UseCompanySecret { get; set; }

        public DateTime CreatedAt { get; set; }

        public string SecretPlaintext { get; set; } = default!;
    }
}
