using LYRA.Server.Models.Logging;
using LYRA.Server.Services.Logging.Interfaces;
using System.Threading.Channels;

namespace LYRA.Server.Services.Logging
{
	/// <summary>
	/// In-memory implementation of <see cref="ILogQueue"/> based on <see cref="Channel{T}"/>.
	/// Provides a thread-safe, high-performance queue for transporting structured log entries
	/// from the main execution path (e.g., request handlers) to background log writers.
	/// 
	/// The channel decouples logging from persistence, ensuring that logs are handled
	/// asynchronously without blocking the caller.
	/// </summary>
	public class InMemoryLogQueue : ILogQueue
	{
		private readonly Channel<LogEntryDto> _channel;

		/// <summary>
		/// Initializes an unbounded channel for log entry transmission.
		/// Unbounded channel ensures no log is dropped due to buffer limits,
		/// but care should be taken to prevent memory overflow under heavy load.
		/// </summary>
		public InMemoryLogQueue()
		{
			_channel = Channel.CreateUnbounded<LogEntryDto>();
		}

		/// <summary>
		/// Enqueues a structured log entry into the channel for asynchronous processing.
		/// </summary>
		/// <param name="log">The log entry to enqueue.</param>
		public void Enqueue(LogEntryDto log)
		{
			_channel.Writer.TryWrite(log);
		}

		/// <summary>
		/// Exposes the reader for background consumers (e.g., <see cref="BackgroundLogWriterService"/>).
		/// </summary>
		public ChannelReader<LogEntryDto> Reader => _channel.Reader;
	}
}
