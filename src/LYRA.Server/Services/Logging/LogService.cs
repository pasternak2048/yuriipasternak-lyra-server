using LYRA.Server.Data.LyraLogsDb;
using LYRA.Server.Entities.Logging;
using LYRA.Server.Hubs;
using LYRA.Server.Services.Logging.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LYRA.Server.Services.Logging
{
    /// <summary>
    /// Provides structured logging capabilities with optional real-time broadcast.
    /// </summary>
    public class LogService : ILogService
    {
        private readonly LyraLogsDbContext _db;
        private readonly IHubContext<LyraActivityHub>? _hubContext;

        public LogService(LyraLogsDbContext db, IHubContext<LyraActivityHub>? hubContext = null)
        {
            _db = db;
            _hubContext = hubContext;
        }

        /// <inheritdoc/>
        public async Task WriteAsync(
            string type,
            string status,
            string description,
            string? source = null,
            string? exception = null,
            string? callerSystem = null,
            string? targetSystem = null,
            string? signatureHash = null,
            string? metadataJson = null)
        {
            var entry = new LogEntryEntity
            {
                TimestampUtc = DateTime.UtcNow,
                Type = type,
                Status = status,
                Description = description,
                Source = source,
                Exception = exception,
                CallerSystem = callerSystem,
                TargetSystem = targetSystem,
                SignatureHash = signatureHash,
                MetadataJson = metadataJson
            };

            _db.Logs.Add(entry);
            await _db.SaveChangesAsync();

            if (_hubContext is not null)
            {
                var formatted = $"[{entry.TimestampUtc:HH:mm:ss}] [{status}] {type}: {description}";
                await _hubContext.Clients.All.SendAsync("ReceiveLog", formatted);
            }
        }
    }
}
