using LYRA.Server.Services.Logging.Interfaces;

namespace LYRA.Server.Services.Logging
{
	/// <summary>
	/// Background service that continuously reads structured log entries from the in-memory log queue
	/// and writes them to the persistent storage (e.g., SQL database) using <see cref="ILogService"/>.
	/// 
	/// This decouples the critical request-processing flow from the cost of logging,
	/// ensuring that logging operations do not delay responses.
	/// 
	/// The service uses scoped DI resolution to avoid lifetime issues
	/// and supports cancellation via <see cref="CancellationToken"/>.
	/// </summary>
	public class BackgroundLogWriterService : BackgroundService
	{
		private readonly InMemoryLogQueue _queue;
		private readonly IServiceProvider _provider;
		private readonly ILogger<BackgroundLogWriterService> _logger;

		/// <summary>
		/// Constructs the background log writer with DI dependencies.
		/// </summary>
		/// <param name="provider">Root service provider used to create scoped services (e.g., DbContext, ILogService).</param>
		/// <param name="queue">Shared log queue containing log entries to be persisted.</param>
		/// <param name="logger">Logger for internal error reporting.</param>
		public BackgroundLogWriterService(
			IServiceProvider provider,
			ILogQueue queue,
			ILogger<BackgroundLogWriterService> logger)
		{
			_provider = provider;
			_queue = (InMemoryLogQueue)queue;
			_logger = logger;
		}

		/// <summary>
		/// Continuously listens for new log entries and writes them to persistent storage.
		/// Runs until the application is stopped or the token is cancelled.
		/// </summary>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await foreach (var log in _queue.Reader.ReadAllAsync(stoppingToken))
			{
				try
				{
					// Create a new scoped lifetime to resolve ILogService and DbContext safely.
					using var scope = _provider.CreateScope();
					var writer = scope.ServiceProvider.GetRequiredService<ILogService>();

					await writer.WriteAsync(
						type: log.Type,
						status: log.Status,
						description: log.Description,
						callerSystem: log.CallerSystem,
						targetSystem: log.TargetSystem,
						signatureHash: log.SignatureHash,
						source: log.Source,
						exception: log.Exception);
				}
				catch (Exception ex)
				{
					// Logs failure to persist a log entry (does not break the loop).
					_logger.LogError(ex, "Failed to write log");
				}
			}
		}
	}
}
