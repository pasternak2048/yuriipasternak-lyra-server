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
        /// DbSet for access policy rules.
        /// </summary>
        public DbSet<AccessPolicyRuleEntity> AccessPolicyRules => Set<AccessPolicyRuleEntity>();

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
                entity.Property(c => c.SystemName)
                      .HasConversion(v => v.ToLowerInvariant(), v => v)
                      .IsUnicode(false)
                      .HasMaxLength(100);

                entity.Property(c => c.DisplayName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.IsDeleted)
                      .HasDefaultValue(false);

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(c => c.CreatedBy).IsRequired(false);
                entity.Property(c => c.ModifiedAt).IsRequired(false);
                entity.Property(c => c.ModifiedBy).IsRequired(false);

                entity.HasMany(c => c.TrustedTouchpoints)
                      .WithOne(t => t.Company)
                      .HasForeignKey(t => t.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.SystemName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0 AND [IsActive] = 1")
                      .HasDatabaseName("IX_Companies_SystemName_Active");
            });

            // ------------------- TrustedTouchpoint -------------------
            modelBuilder.Entity<TrustedTouchpointEntity>(entity =>
            {
                entity.Property(t => t.SystemName)
                      .HasConversion(v => v.ToLowerInvariant(), v => v)
                      .IsUnicode(false)
                      .HasMaxLength(100);

                entity.Property(t => t.DisplayName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.SignatureType)
                      .HasConversion<string>();

                entity.Property(t => t.IsDeleted)
                      .HasDefaultValue(false);

                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(t => t.CreatedBy).IsRequired(false);
                entity.Property(t => t.ModifiedAt).IsRequired(false);
                entity.Property(t => t.ModifiedBy).IsRequired(false);

                entity.HasMany(t => t.OutgoingPolicies)
                      .WithOne(p => p.Caller)
                      .HasForeignKey(p => p.CallerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(t => t.IncomingPolicies)
                      .WithOne(p => p.Target)
                      .HasForeignKey(p => p.TargetId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.SystemName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0 AND [IsActive] = 1")
                      .HasDatabaseName("IX_TrustedTouchpoints_SystemName_Active");
            });

            // ------------------- AccessPolicy -------------------
            modelBuilder.Entity<AccessPolicyEntity>(entity =>
            {
                entity.Property(p => p.CallerSystemName)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(100)
                      .HasConversion(v => v.ToLowerInvariant(), v => v);

                entity.Property(p => p.TargetSystemName)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(100)
                      .HasConversion(v => v.ToLowerInvariant(), v => v);

                entity.Property(p => p.IsEnabled)
                      .HasDefaultValue(true);

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(p => p.CreatedBy).IsRequired(false);
                entity.Property(p => p.ModifiedAt).IsRequired(false);
                entity.Property(p => p.ModifiedBy).IsRequired(false);

                entity.HasIndex(p => new
                {
                    p.CallerSystemName,
                    p.TargetSystemName
                })
                .IsUnique()
                .HasDatabaseName("IX_AccessPolicy_Key");

                entity.HasMany(p => p.Rules)
                      .WithOne(r => r.AccessPolicy)
                      .HasForeignKey(r => r.AccessPolicyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------- AccessPolicyRule -------------------
            modelBuilder.Entity<AccessPolicyRuleEntity>(entity =>
            {
                entity.Property(r => r.HttpMethod)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(20)
                      .HasConversion(v => v.ToUpperInvariant(), v => v);

                entity.Property(r => r.PathPattern)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(500)
                      .HasConversion(v => v.ToLowerInvariant(), v => v);

                entity.HasIndex(r => new
                {
                    r.AccessPolicyId,
                    r.HttpMethod,
                    r.PathPattern
                })
                .IsUnique()
                .HasDatabaseName("IX_AccessPolicyRule_Unique");
            });
        }
    }
}
