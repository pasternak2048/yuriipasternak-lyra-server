using LYRA.Server.Models.Logging;

namespace LYRA.Server.Services.Logging.Interfaces
{
    /// <summary>
    /// Defines contract for writing structured log entries to the logging store.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Writes a new log entry to the log store.
        /// </summary>
        /// <param name="type">The high-level category of the log (e.g., "Verification", "System").</param>
        /// <param name="status">The status of the event (e.g., "Success", "Fail").</param>
        /// <param name="description">Short summary of the event.</param>
        /// <param name="source">Optional source (e.g., service or class name).</param>
        /// <param name="exception">Optional exception message or stack trace.</param>
        /// <param name="callerSystem">Optional caller system name.</param>
        /// <param name="targetSystem">Optional target system name.</param>
        /// <param name="signatureHash">Optional hash or signature involved.</param>
        /// <param name="metadataJson">Optional structured metadata as JSON.</param>
        Task WriteAsync(
            string type,
            string status,
            string description,
            string? source = null,
            string? exception = null,
            string? callerSystem = null,
            string? targetSystem = null,
            string? signatureHash = null,
            string? metadataJson = null);

        /// <summary>
        /// Retrieves the most recent log entries, ordered by timestamp descending.
        /// </summary>
        /// <param name="limit">Maximum number of logs to return.</param>
        /// <returns>List of recent log entries formatted for display.</returns>
        Task<List<LogEntryDto>> GetRecentAsync(int limit = 100);

        /// <summary>
        /// Retrieves the total number of logs in the system.
        /// </summary>
        /// <returns>The total count of logs.</returns>
        Task<int> GetTotalLogsCountAsync();
    }
}
