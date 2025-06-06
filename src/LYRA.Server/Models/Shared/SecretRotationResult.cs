using LYRA.Security.Enums;

namespace LYRA.Server.Models.Shared
{
    /// <summary>
    /// Represents the result of a secret rotation operation for a company or trusted touchpoint.
    /// </summary>
    public class SecretRotationResult
    {
        /// <summary>
        /// The unique identifier of the entity whose secret was rotated.
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Indicates whether the rotated secret belongs to a company or a trusted touchpoint.
        /// </summary>
        public SecretOwnerType OwnerType { get; set; }

        /// <summary>
        /// The newly generated plaintext secret (only shown once).
        /// </summary>
        public string SecretPlaintext { get; set; } = default!;
    }
}