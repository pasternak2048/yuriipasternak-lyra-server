using LYRA.Server.Enums;

namespace LYRA.Server.Models.Verify
{
    public class VerifyRequest
    {
        public string Caller { get; set; } = null!;

        public string Target { get; set; } = null!;

        public string Method { get; set; } = null!;

        public string Path { get; set; } = null!;

        public string PayloadHash { get; set; } = null!;

        public string Timestamp { get; set; } = null!;

        public AccessContext Context { get; set; }

        public string Signature { get; set; } = null!;
    }
}