using LYRA.Server.Enums;
using LYRA.Server.Models.Verify;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Metadata for an access context signature rule, such as whether payload hash verification is required.
    /// </summary>
    public class SignatureContextMetadata
    {
        /// <summary>
        /// The access context this metadata applies to.
        /// </summary>
        public AccessContext Context { get; set; }

        /// <summary>
        /// Function that determines if a payload hash is required for a given request.
        /// </summary>
        public Func<VerifyRequest, bool> RequiresPayloadHash { get; set; } = _ => false;
    }
}
