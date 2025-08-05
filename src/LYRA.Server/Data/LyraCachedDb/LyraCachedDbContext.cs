using LYRA.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Data.LyraCachedDb
{
    /// <summary>
    /// DbContext for the LYRA access policy cache database.
    /// Used to store denormalized, performance-optimized access rules.
    /// </summary>
    public class LyraCachedDbContext : DbContext
    {
        public LyraCachedDbContext(DbContextOptions<LyraCachedDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Cached access policies.
        /// </summary>
        public DbSet<CachedAccessPolicyEntity> CachedAccessPolicies => Set<CachedAccessPolicyEntity>();

        /// <summary>
        /// Configures the database schema for the LYRA Cached Access Policy context.
        /// Defines the compound primary key, indexes, and property constraints for the CachedAccessPolicyEntity.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CachedAccessPolicyEntity>(entity =>
            {
                // Define table name explicitly (optional if default naming used)
                entity.ToTable("CachedAccessPolicies");

                // -------------------- PRIMARY KEY --------------------
                // Use key to ensure uniqueness and identity tracking
                entity.HasKey(p => p.Id);

                // -------------------- UNIQUE INDEX --------------------
                // Optional but improves query performance
                entity.HasIndex(p => new
                {
                    p.CallerSystemName,
                    p.TargetSystemName
                })
                .IsUnique()
                .HasDatabaseName("IX_CachedAccessPolicy_Key");

                // -------------------- Property Configuration --------------------

                // System name of the caller (touchpoint)
                entity.Property(p => p.CallerSystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                // System name of the target (touchpoint)
                entity.Property(p => p.TargetSystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Operation name (e.g., POST /api/...) — always lowercase
                entity.Property(p => p.Operation)
                      .IsRequired()
                      .HasMaxLength(2000);

                // Signature type used for validation (HMAC / RSA)
                entity.Property(p => p.SignatureType)
                      .IsRequired()
                      .HasMaxLength(50);

                // Secret key used for signing (encrypted in DB)
                entity.Property(p => p.CallerSecret)
                      .IsRequired();

                // Optional IP restriction (CIDR or single IP)
                entity.Property(p => p.AllowedSourceIp)
                      .HasMaxLength(100);

                // Company slug (SystemName) for the caller
                entity.Property(p => p.CallerCompanySystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Company slug (SystemName) for the target
                entity.Property(p => p.TargetCompanySystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Optional: Timestamp when the cache was generated
                entity.Property(p => p.CachedAtUtc);
            });
        }
    }
}
