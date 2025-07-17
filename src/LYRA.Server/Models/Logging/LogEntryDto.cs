namespace LYRA.Server.Models.Logging
{
	/// <summary>
	/// Represents a lightweight view model for displaying structured log entries in the UI.
	/// Designed for real-time dashboards, SignalR log consoles, and also supports background write queues.
	/// </summary>
	public class LogEntryDto
	{
		/// <summary>
		/// Local or formatted time for display (e.g., HH:mm:ss).  
		/// This is typically set on the UI side or formatting layer.
		/// </summary>
		public string Timestamp { get; set; } = default!;

		/// <summary>
		/// High-level category of the log event (e.g., Verification, System, Security).
		/// </summary>
		public string Type { get; set; } = default!;

		/// <summary>
		/// Status of the log entry (e.g., Success, Fail, Info, Warning, Critical).
		/// </summary>
		public string Status { get; set; } = default!;

		/// <summary>
		/// Short description of the log event. This is the main message shown to the user.
		/// </summary>
		public string Description { get; set; } = default!;

		/// <summary>
		/// Optional source of the log — class name, service name, etc.
		/// </summary>
		public string? Source { get; set; }

		/// <summary>
		/// Optional color class used for status highlighting in the UI (e.g., text-success).
		/// </summary>
		public string StatusColor { get; set; } = "text-secondary";

		/// <summary>
		/// Optional name of the calling system (typically a TrustedTouchpoint).
		/// </summary>
		public string? CallerSystem { get; set; }

		/// <summary>
		/// Optional name of the target system (typically a TrustedTouchpoint).
		/// </summary>
		public string? TargetSystem { get; set; }

		/// <summary>
		/// Optional hash or signature string related to the verified request.
		/// </summary>
		public string? SignatureHash { get; set; }

		/// <summary>
		/// Optional exception or stack trace (included only for error/debug logs).
		/// </summary>
		public string? Exception { get; set; }
	}
}
