using LYRA.Server.Data.LyraLogsDb;
using LYRA.Server.Entities.Logging;
using LYRA.Server.Hubs;
using LYRA.Server.Models.Logging;
using LYRA.Server.Services.Logging.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
                var dto = new LogEntryDto
                {
                    Timestamp = entry.TimestampUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                    Type = entry.Type,
                    Status = entry.Status ?? "Info",
                    Description = entry.Description,
                    Source = entry.Source,
                    CallerSystem = entry.CallerSystem ?? "",
                    TargetSystem = entry.TargetSystem ?? "",
                    SignatureHash = entry.SignatureHash ?? "",
                    StatusColor = (entry.Status?.ToLower()) switch
                    {
                        "success" => "text-success",
                        "fail" or "error" => "text-danger",
                        "warning" => "text-warning",
                        "critical" => "text-danger fw-bold",
                        _ => "text-secondary"
                    }
                };

                await _hubContext.Clients.All.SendAsync("ReceiveLog", dto);
            }
        }

        /// <inheritdoc/>
        public async Task<List<LogEntryDto>> GetRecentAsync(int limit = 100)
        {
            var logs = await _db.Logs
                .AsNoTracking()
                .OrderByDescending(l => l.TimestampUtc)
                .Take(limit)
                .ToListAsync();

            return logs.Select(l => new LogEntryDto
            {
                Timestamp = l.TimestampUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                Type = l.Type,
                Status = l.Status ?? "Info",
                Description = l.Description,
                Source = l.Source,
                CallerSystem = l.CallerSystem ?? "",
                TargetSystem = l.TargetSystem ?? "",
                SignatureHash = l.SignatureHash ?? "",
                StatusColor = (l.Status?.ToLower()) switch
                {
                    "success" => "text-success",
                    "fail" or "error" => "text-danger",
                    "warning" => "text-warning",
                    "critical" => "text-danger fw-bold",
                    _ => "text-secondary"
                }
            }).ToList();
        }

        /// <inheritdoc />
        public async Task<int> GetTotalLogsCountAsync()
        {
            return await _db.Logs.AsNoTracking().CountAsync();
        }
    }
}
