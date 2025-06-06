using LYRA.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Verify
{
    /// <summary>
    /// Represents a request to verify a signed interaction between two trusted touchpoints.
    /// </summary>
    public class VerifyRequest
    {
        /// <summary>
        /// System name of the initiating touchpoint (e.g., "gateway@bcorp").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Caller { get; set; } = null!;

        /// <summary>
        /// System name of the receiving touchpoint (e.g., "billing@acorp").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Target { get; set; } = null!;

        /// <summary>
        /// HTTP method or action type (e.g., "POST", "GET").
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Method { get; set; } = null!;

        /// <summary>
        /// Request path or resource identifier (e.g., "/subscribe").
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Path { get; set; } = null!;

        /// <summary>
        /// Raw payload data (e.g., JSON body or serialized content).
        /// Required to recompute and verify the PayloadHash.
        /// </summary>
        public string? Payload { get; set; }

        /// <summary>
        /// Base64-encoded hash of the request payload.
        /// Used for integrity verification.
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string PayloadHash { get; set; } = null!;

        /// <summary>
        /// UTC timestamp at the time the request was signed (Unix format recommended).
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Timestamp { get; set; } = null!;

        /// <summary>
        /// Type of interaction being verified (e.g., Http, Event, Grpc).
        /// </summary>
        [Required]
        [EnumDataType(typeof(AccessContext))]
        public AccessContext Context { get; set; }

        /// <summary>
        /// Base64-encoded signature of the full verification string.
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string Signature { get; set; } = null!;
    }
}