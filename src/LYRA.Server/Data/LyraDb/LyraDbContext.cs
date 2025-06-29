using LYRA.Server.Entities;
using LYRA.Server.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Data.LyraDb
{
    /// <summary>
    /// Represents the Entity Framework Core database context for LYRA server.
    /// Includes identity and custom entities for companies, touchpoints, and access policies.
    /// </summary>
    public class LyraDbContext : IdentityDbContext<ApplicationUser>
    {
        public LyraDbContext(DbContextOptions<LyraDbContext> options) : base(options) { }

        /// <summary>
        /// DbSet for company entities (tenants/organizations).
        /// </summary>
        public DbSet<CompanyEntity> Companies => Set<CompanyEntity>();

        /// <summary>
        /// DbSet for trusted touchpoints belonging to companies.
        /// </summary>
        public DbSet<TrustedTouchpointEntity> TrustedTouchpoints => Set<TrustedTouchpointEntity>();

        /// <summary>
        /// DbSet for access policies defining permissions between touchpoints.
        /// </summary>
        public DbSet<AccessPolicyEntity> AccessPolicies => Set<AccessPolicyEntity>();

        /// <summary>
        /// Configures the entity mappings and constraints for the model.
        /// Sets up indexes, property conversions, and relationships.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------- Company -------------------
            modelBuilder.Entity<CompanyEntity>(entity =>
            {
                // Normalize SystemName to lowercase, store as varchar(100)
                entity.Property(c => c.SystemName)
                      .HasConversion(v => v.ToLowerInvariant(), v => v)
                      .IsUnicode(false)
                      .HasMaxLength(100);

                // DisplayName is required and max 200 chars
                entity.Property(c => c.DisplayName)
                      .IsRequired()
                      .HasMaxLength(200);

                // Soft delete flag defaults to false
                entity.Property(c => c.IsDeleted)
                      .HasDefaultValue(false);

                // CreatedAt defaults to current UTC timestamp by database
                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Audit properties are optional
                entity.Property(c => c.CreatedBy).IsRequired(false);
                entity.Property(c => c.ModifiedAt).IsRequired(false);
                entity.Property(c => c.ModifiedBy).IsRequired(false);

                // One-to-many relation with TrustedTouchpoints
                entity.HasMany(c => c.TrustedTouchpoints)
                      .WithOne(t => t.Company)
                      .HasForeignKey(t => t.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Unique filtered index on SystemName (only active, not deleted)
                entity.HasIndex(c => c.SystemName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0 AND [IsActive] = 1")
                      .HasDatabaseName("IX_Companies_SystemName_Active");
            });

            // ------------------- TrustedTouchpoint -------------------
            modelBuilder.Entity<TrustedTouchpointEntity>(entity =>
            {
                // Normalize SystemName to lowercase, store as varchar(100)
                entity.Property(t => t.SystemName)
                      .HasConversion(v => v.ToLowerInvariant(), v => v)
                      .IsUnicode(false)
                      .HasMaxLength(100);

                // DisplayName is required and max 200 chars
                entity.Property(t => t.DisplayName)
                      .IsRequired()
                      .HasMaxLength(200);

                // Enum conversions to strings
                entity.Property(t => t.Mode)
                      .HasConversion<string>();

                entity.Property(t => t.SignatureType)
                      .HasConversion<string>();

                // Soft delete flag defaults to false
                entity.Property(t => t.IsDeleted)
                      .HasDefaultValue(false);

                // CreatedAt defaults to current UTC timestamp by database
                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Audit properties are optional
                entity.Property(t => t.CreatedBy).IsRequired(false);
                entity.Property(t => t.ModifiedAt).IsRequired(false);
                entity.Property(t => t.ModifiedBy).IsRequired(false);

                // Navigation properties for outgoing and incoming policies
                entity.HasMany(t => t.OutgoingPolicies)
                      .WithOne(p => p.Caller)
                      .HasForeignKey(p => p.CallerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(t => t.IncomingPolicies)
                      .WithOne(p => p.Target)
                      .HasForeignKey(p => p.TargetId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Unique filtered index on SystemName (only active, not deleted)
                entity.HasIndex(t => t.SystemName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0 AND [IsActive] = 1")
                      .HasDatabaseName("IX_TrustedTouchpoints_SystemName_Active");
            });

            // ------------------- AccessPolicy -------------------
            modelBuilder.Entity<AccessPolicyEntity>(entity =>
            {
                // Caller and Target system names
                entity.Property(p => p.CallerSystemName)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(100);

                entity.Property(p => p.TargetSystemName)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(100);

                // Normalize Operation to lowercase, max 200 chars
                entity.Property(p => p.Operation)
                      .IsRequired()
                      .HasMaxLength(2000)
                      .HasConversion(
                          v => v.ToLowerInvariant(),
                          v => v
                      );

                // Store Context enum as string
                entity.Property(p => p.Context)
                      .HasConversion<string>();

                // IsEnabled defaults to true
                entity.Property(p => p.IsEnabled)
                      .HasDefaultValue(true);

                // CreatedAt defaults to current UTC timestamp
                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Audit properties are optional
                entity.Property(p => p.CreatedBy).IsRequired(false);
                entity.Property(p => p.ModifiedAt).IsRequired(false);
                entity.Property(p => p.ModifiedBy).IsRequired(false);

                // Composite unique index for fast verification
                entity.HasIndex(p => new
                {
                    p.CallerSystemName,
                    p.TargetSystemName,
                    p.Context
                })
                .IsUnique()
                .HasDatabaseName("IX_AccessPolicy_Key");
            });
        }
    }
}
