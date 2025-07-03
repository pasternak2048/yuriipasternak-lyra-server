using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Entities.Logging
{
    /// <summary>
    /// Represents a unified log entry in the system, capturing structured information
    /// about runtime events, including verification attempts, system operations,
    /// exceptions, and other relevant activities. Designed to support both real-time
    /// monitoring and historical auditing through a single, extensible model.
    /// </summary>
    public class LogEntryEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>UTC timestamp of the log entry</summary>
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        /// <summary>High-level category of the event (System, Verification, Security, Storage, etc.)</summary>
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = default!;

        /// <summary>Status of the event: Success, Fail, Warning, Info, Debug, Critical</summary>
        [MaxLength(20)]
        public string? Status { get; set; }

        /// <summary>Short description of the log entry</summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = default!;

        /// <summary>Optional exception message or stack trace</summary>
        public string? Exception { get; set; }

        /// <summary>Optional name of the system or service generating the log</summary>
        [MaxLength(100)]
        public string? Source { get; set; }

        /// <summary>Optional caller system name (e.g., TrustedTouchpoint)</summary>
        [MaxLength(100)]
        public string? CallerSystem { get; set; }

        /// <summary>Optional target system name (e.g., TrustedTouchpoint)</summary>
        [MaxLength(100)]
        public string? TargetSystem { get; set; }

        /// <summary>Optional hash or signature string related to the request</summary>
        [MaxLength(200)]
        public string? SignatureHash { get; set; }

        /// <summary>Serialized metadata (JSON string) for extensibility</summary>
        public string? MetadataJson { get; set; }
    }
}
