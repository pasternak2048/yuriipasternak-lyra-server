using LYRA.Server.Entities;
using LYRA.Server.Entities.Identity;
using LYRA.Server.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<LyraDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            context.Database.EnsureCreated();

            const string adminEmail = "admin@lyra";
            const string adminPassword = "admin";

            // 1. Seed admin user
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // 2. Seed companies: bcorp and acorp
            var bcorp = await EnsureCompanyAsync(context, "bcorp", "B Corp", "bcorp-secret");
            var acorp = await EnsureCompanyAsync(context, "acorp", "A Corp", "acorp-secret");

            // 3. Seed agents
            var gateway = await EnsureAgentAsync(context, bcorp, "gateway", "gateway-secret", AgentMode.CallerOnly);
            var billing = await EnsureAgentAsync(context, acorp, "billing", "billing-secret", AgentMode.TargetOnly);

            // 4. Seed policy: bcorp::gateway → acorp::billing
            if (!context.AccessPolicies.Any(p =>
                p.CallerAgentId == gateway.Id && p.TargetAgentId == billing.Id &&
                p.Method == "POST" && p.PathPattern == "/subscribe"))
            {
                context.AccessPolicies.Add(new AccessPolicyEntity
                {
                    Id = Guid.NewGuid(),
                    CallerAgentId = gateway.Id,
                    TargetAgentId = billing.Id,
                    Method = "POST",
                    PathPattern = "/subscribe",
                    IsEnabled = true,
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }

        // Helpers
        private static async Task<CompanyEntity> EnsureCompanyAsync(
            LyraDbContext context,
            string name,
            string displayName,
            string secret)
        {
            var existing = await context.Companies.FirstOrDefaultAsync(c => c.Name == name);
            if (existing != null) return existing;

            var company = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                Name = name,
                DisplayName = displayName,
                Secret = secret,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Companies.Add(company);
            await context.SaveChangesAsync();

            return company;
        }

        private static async Task<TrustedAgentEntity> EnsureAgentAsync(
            LyraDbContext context,
            CompanyEntity company,
            string name,
            string secret,
            AgentMode mode)
        {
            var existing = await context.TrustedAgents
                .FirstOrDefaultAsync(a => a.CompanyId == company.Id && a.Name == name);

            if (existing != null) return existing;

            var agent = new TrustedAgentEntity
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                Name = name,
                Secret = secret,
                UseCompanySecret = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Mode = mode
            };

            context.TrustedAgents.Add(agent);
            await context.SaveChangesAsync();

            return agent;
        }
    }
}
