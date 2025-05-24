using LYRA.Server.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Data
{
    public class LyraDbContext : IdentityDbContext<ApplicationUser>
    {
        public LyraDbContext(DbContextOptions<LyraDbContext> options) : base(options) { }

        public DbSet<TrustedService> TrustedServices => Set<TrustedService>();

        public DbSet<AccessPolicy> AccessPolicies => Set<AccessPolicy>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TrustedService>()
                .HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();

            modelBuilder.Entity<AccessPolicy>()
                .HasIndex(x => new { x.CompanyId, x.Caller, x.Target, x.Method, x.PathPattern });
        }
    }
}
