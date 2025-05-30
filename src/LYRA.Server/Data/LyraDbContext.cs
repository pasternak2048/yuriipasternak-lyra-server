using LYRA.Server.Entities;
using LYRA.Server.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Data
{
    public class LyraDbContext : IdentityDbContext<ApplicationUser>
    {
        public LyraDbContext(DbContextOptions<LyraDbContext> options) : base(options) { }

        public DbSet<CompanyEntity> Companies=> Set<CompanyEntity>();

        public DbSet<TrustedTouchpointEntity> TrustedTouchpoints => Set<TrustedTouchpointEntity>();

        public DbSet<AccessPolicyEntity> AccessPolicies => Set<AccessPolicyEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------- Company -------------------

            modelBuilder.Entity<CompanyEntity>(entity =>
            {
                entity.HasIndex(c => c.SystemName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");

                entity.Property(c => c.SystemName)
                      .HasConversion(v => v.ToLowerInvariant(), v => v)
                      .IsUnicode(false)
                      .HasColumnType("varchar(100)");

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
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------- TrustedTouchpoint -------------------

            modelBuilder.Entity<TrustedTouchpointEntity>(entity =>
            {
                entity.HasIndex(t => t.SystemName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");

                entity.Property(t => t.SystemName)
                      .HasConversion(v => v.ToLowerInvariant(), v => v)
                      .IsUnicode(false)
                      .HasColumnType("varchar(100)");

                entity.Property(t => t.DisplayName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Mode)
                      .HasConversion<string>();

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
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.IncomingPolicies)
                      .WithOne(p => p.Target)
                      .HasForeignKey(p => p.TargetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------- AccessPolicy -------------------

            modelBuilder.Entity<AccessPolicyEntity>(entity =>
            {
                entity.HasIndex(p => new
                {
                    p.CallerSystemName,
                    p.TargetSystemName,
                    p.Context,
                    p.Operation
                }).IsUnique();

                entity.Property(p => p.CallerSystemName)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(100)
                      .HasColumnType("varchar(100)");

                entity.Property(p => p.TargetSystemName)
                      .IsRequired()
                      .IsUnicode(false)
                      .HasMaxLength(100)
                      .HasColumnType("varchar(100)");

                entity.Property(p => p.Operation)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Context)
                      .HasConversion<string>();

                entity.Property(p => p.IsEnabled)
                      .HasDefaultValue(true);

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(p => p.CreatedBy).IsRequired(false);
                entity.Property(p => p.ModifiedAt).IsRequired(false);
                entity.Property(p => p.ModifiedBy).IsRequired(false);
            });
        }
    }
}
