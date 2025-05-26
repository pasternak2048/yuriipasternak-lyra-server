using LYRA.Server.Enums;

namespace LYRA.Server.Models.Shared
{
    public class SecretRotationResult
    {
        public Guid EntityId { get; set; }

        public SecretOwnerType OwnerType { get; set; }

        public string SecretPlaintext { get; set; } = default!;
    }
}