namespace LYRA.Server.Models.TrustedTouchpoint
{
    public class TrustedTouchpointCreatedDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string DisplayName { get; set; } = default!;

        public string SecretPlaintext { get; set; } = default!;
    }
}
