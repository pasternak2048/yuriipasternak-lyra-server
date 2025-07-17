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
		/// <summary>
		/// Unique identifier of the log entry.
		/// </summary>
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// UTC timestamp when the log entry was created.
		/// </summary>
		public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// High-level category of the event (e.g., Verification, System, Security).
		/// </summary>
		[Required]
		[MaxLength(50)]
		public string Type { get; set; } = default!;

		/// <summary>
		/// Status of the event (e.g., Success, Fail, Warning, Info, Critical).
		/// </summary>
		[MaxLength(20)]
		public string? Status { get; set; }

		/// <summary>
		/// Short description of what happened in this event.
		/// </summary>
		[Required]
		[MaxLength(500)]
		public string Description { get; set; } = default!;

		/// <summary>
		/// Optional exception message or full stack trace if an error occurred.
		/// </summary>
		public string? Exception { get; set; }

		/// <summary>
		/// Optional source (e.g., service name or class) that generated the log entry.
		/// </summary>
		[MaxLength(100)]
		public string? Source { get; set; }

		/// <summary>
		/// Optional name of the calling system (typically a TrustedTouchpoint).
		/// </summary>
		[MaxLength(100)]
		public string? CallerSystem { get; set; }

		/// <summary>
		/// Optional name of the target system (typically a TrustedTouchpoint).
		/// </summary>
		[MaxLength(100)]
		public string? TargetSystem { get; set; }

		/// <summary>
		/// Optional signature or hash related to the request or payload.
		/// </summary>
		[MaxLength(200)]
		public string? SignatureHash { get; set; }

		/// <summary>
		/// Optional metadata in serialized JSON format for extensibility.
		/// </summary>
		public string? MetadataJson { get; set; }
	}
}
