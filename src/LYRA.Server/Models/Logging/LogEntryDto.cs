namespace LYRA.Server.Models.Logging
{
    /// <summary>
    /// Represents a lightweight view model for displaying structured log entries in the UI.
    /// Designed for real-time dashboards and log consoles.
    /// </summary>
    public class LogEntryDto
    {
        /// <summary>
        /// Local or formatted time for display (e.g. HH:mm:ss).
        /// </summary>
        public string Timestamp { get; set; } = default!;

        /// <summary>
        /// Category of the log (e.g., Verification, System, Security).
        /// </summary>
        public string Type { get; set; } = default!;

        /// <summary>
        /// Status of the log entry (e.g., Success, Fail, Info).
        /// </summary>
        public string Status { get; set; } = default!;

        /// <summary>
        /// Short description of the log event.
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// Optional source that generated the log (e.g., service or class).
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Optional color class for visual status indicator (e.g., text-success).
        /// </summary>
        public string StatusColor { get; set; } = "text-secondary";

        /// <summary>
        /// Optional caller system name
        /// </summary>
        public string? CallerSystem { get; set; }

        /// <summary>
        /// Optional target system name
        /// </summary>
        public string? TargetSystem { get; set; }

        /// <summary>
        /// Optional hash or signature
        /// </summary>
        public string? SignatureHash { get; set; }
    }
}
