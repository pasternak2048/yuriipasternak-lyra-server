using LYRA.Server.Models.Logging;

namespace LYRA.Server.Services.Logging.Interfaces
{
	/// <summary>
	/// Represents a lightweight in-memory log queue interface used to decouple
	/// real-time log generation from database persistence.
	/// 
	/// Log entries are enqueued immediately (e.g., during request processing),
	/// and later consumed asynchronously by a background service (e.g., <see cref="BackgroundLogWriterService"/>).
	/// 
	/// This approach prevents log persistence from slowing down critical execution paths,
	/// especially in high-performance scenarios like request verification.
	/// </summary>
	public interface ILogQueue
	{
		/// <summary>
		/// Enqueues a log entry into the in-memory channel for asynchronous processing.
		/// </summary>
		/// <param name="log">Structured log entry to be written to storage.</param>
		void Enqueue(LogEntryDto log);
	}
}
