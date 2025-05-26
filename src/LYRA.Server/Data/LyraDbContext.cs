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

            modelBuilder.Entity<CompanyEntity>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<CompanyEntity>()
                .Property(c => c.Name)
                .HasConversion(
                    v => v.ToLowerInvariant(),
                    v => v
                )
                .IsUnicode(false)
                .HasColumnType("varchar(100)");

            modelBuilder.Entity<CompanyEntity>()
                .Property(c => c.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<CompanyEntity>()
                .HasMany(c => c.TrustedTouchpoints)
                .WithOne(t => t.Company)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------- TrustedTouchpoint -------------------

            modelBuilder.Entity<TrustedTouchpointEntity>()
                .HasIndex(t => new { t.CompanyId, t.Name }) // Ensure slug is unique within company
                .IsUnique();

            modelBuilder.Entity<TrustedTouchpointEntity>()
                .Property(t => t.Name)
                .HasConversion(
                    v => v.ToLowerInvariant(),
                    v => v
                )
                .IsUnicode(false)
                .HasColumnType("varchar(100)");

            modelBuilder.Entity<TrustedTouchpointEntity>()
                .Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<TrustedTouchpointEntity>()
                .Property(t => t.Mode)
                .HasConversion<string>();

            modelBuilder.Entity<TrustedTouchpointEntity>()
                .Property(t => t.SignatureType)
                .HasConversion<string>();

            modelBuilder.Entity<TrustedTouchpointEntity>()
                .HasMany(t => t.OutgoingPolicies)
                .WithOne(p => p.Caller)
                .HasForeignKey(p => p.CallerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrustedTouchpointEntity>()
                .HasMany(t => t.IncomingPolicies)
                .WithOne(p => p.Target)
                .HasForeignKey(p => p.TargetId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------- AccessPolicy -------------------

            modelBuilder.Entity<AccessPolicyEntity>()
                .HasIndex(p => new
                {
                    p.CallerId,
                    p.TargetId,
                    p.Context,
                    p.Operation
                }).IsUnique();

            modelBuilder.Entity<AccessPolicyEntity>()
                .Property(p => p.Context)
                .HasConversion<string>();
        }
    }
}
