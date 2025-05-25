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

        public DbSet<TrustedAgentEntity> TrustedAgents => Set<TrustedAgentEntity>();

        public DbSet<AccessPolicyEntity> AccessPolicies => Set<AccessPolicyEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CompanyEntity>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<CompanyEntity>()
                .Property(c => c.Name)
                .HasConversion(
                    v => v.ToLowerInvariant(),
                    v => v
                );

            modelBuilder.Entity<TrustedAgentEntity>()
                .HasIndex(a => new { a.CompanyId, a.Name })
                .IsUnique();

            modelBuilder.Entity<CompanyEntity>()
                .HasMany(c => c.Agents)
                .WithOne(a => a.Company)
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrustedAgentEntity>()
                .HasMany(a => a.OutgoingPolicies)
                .WithOne(p => p.CallerAgent)
                .HasForeignKey(p => p.CallerAgentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrustedAgentEntity>()
                .HasMany(a => a.IncomingPolicies)
                .WithOne(p => p.TargetAgent)
                .HasForeignKey(p => p.TargetAgentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrustedAgentEntity>()
                .Property(a => a.Mode)
                .HasConversion<string>();

            modelBuilder.Entity<AccessPolicyEntity>()
                .HasIndex(p => new {
                    p.CallerAgentId,
                    p.TargetAgentId,
                    p.Method,
                    p.PathPattern
                }).IsUnique();
        }
    }
}
