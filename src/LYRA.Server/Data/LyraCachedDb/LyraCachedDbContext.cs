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
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CachedAccessPolicyEntity>(entity =>
            {
                entity.ToTable("CachedAccessPolicies");

                entity.HasKey(p => p.Id);

                entity.HasIndex(p => new
                {
                    p.CallerSystemName,
                    p.TargetSystemName
                })
                .IsUnique()
                .HasDatabaseName("IX_CachedAccessPolicy_Key");

                entity.Property(p => p.CallerSystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.TargetSystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.RulesJson)
                      .IsRequired();

                entity.Property(p => p.SignatureType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(p => p.CallerSecret)
                      .IsRequired();

                entity.Property(p => p.AllowedSourceIp)
                      .HasMaxLength(100);

                entity.Property(p => p.CallerCompanySystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.TargetCompanySystemName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.CachedAtUtc);
            });
        }
    }
}
