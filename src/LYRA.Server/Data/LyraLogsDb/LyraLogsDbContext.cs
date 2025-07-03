using LYRA.Server.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Data.LyraLogsDb
{
    /// <summary>
    /// Represents the EF Core database context for system log entries.
    /// This context is isolated from the main application context to avoid interference with core operations.
    /// </summary>
    public class LyraLogsDbContext : DbContext
    {
        public LyraLogsDbContext(DbContextOptions<LyraLogsDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Log entries written by the system (verification events, system actions, exceptions).
        /// </summary>
        public DbSet<LogEntryEntity> Logs => Set<LogEntryEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LogEntryEntity>(entity =>
            {
                entity.ToTable("LogEntries");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(20);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Source)
                    .HasMaxLength(100);

                entity.Property(e => e.CallerSystem)
                    .HasMaxLength(100);

                entity.Property(e => e.TargetSystem)
                    .HasMaxLength(100);

                entity.Property(e => e.SignatureHash)
                    .HasMaxLength(200);

                entity.Property(e => e.MetadataJson);
            });
        }
    }
}
