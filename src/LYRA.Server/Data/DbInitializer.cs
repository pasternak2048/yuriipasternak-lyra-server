using LYRA.Server.Entities;
using LYRA.Server.Entities.Identity;
using LYRA.Server.Enums;
using LYRA.Server.Utilities;
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

            // 1. Admin user
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

            // 2. Companies
            var aCorp = await EnsureCompanyAsync(context, "acorp", "A Corp", "a-secret");
            var bCorp = await EnsureCompanyAsync(context, "bcorp", "B Corp", "b-secret");
            var cCorp = await EnsureCompanyAsync(context, "ccorp", "C Corp", "c-secret");

            // 3. Touchpoints per company
            var aBilling = await EnsureTouchpointAsync(context, aCorp, "Billing API", "a-billing-secret", TouchpointMode.TargetOnly);
            var aPublicApi = await EnsureTouchpointAsync(context, aCorp, "Public API", "a-api-secret", TouchpointMode.TargetOnly);

            var bGateway = await EnsureTouchpointAsync(context, bCorp, "Gateway", "b-gateway-secret", TouchpointMode.CallerOnly);
            var bReport = await EnsureTouchpointAsync(context, bCorp, "Report Bot", "b-report-secret", TouchpointMode.Both);

            var cWorker = await EnsureTouchpointAsync(context, cCorp, "Worker Node", "c-worker-secret", TouchpointMode.CallerOnly);
            var cBot = await EnsureTouchpointAsync(context, cCorp, "Bot Commander", "c-bot-secret", TouchpointMode.Both);

            // 4. Example Policy: bCorp::gateway → aCorp::billing (HTTP)
            await EnsurePolicyAsync(context, bGateway.Id, aBilling.Id, "POST /subscribe", AccessContext.Http);
        }

        // Company
        private static async Task<CompanyEntity> EnsureCompanyAsync(LyraDbContext context, string name, string displayName, string secret)
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

        // TrustedTouchpoint
        private static async Task<TrustedTouchpointEntity> EnsureTouchpointAsync(
            LyraDbContext context,
            CompanyEntity company,
            string displayName,
            string secret,
            TouchpointMode mode)
        {
            var name = SlugHelper.Slugify(displayName);

            var existing = await context.TrustedTouchpoints
                .FirstOrDefaultAsync(t => t.CompanyId == company.Id && t.Name == name);

            if (existing != null) return existing;

            var touchpoint = new TrustedTouchpointEntity
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                Name = name,
                DisplayName = displayName,
                Secret = secret,
                UseCompanySecret = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Mode = mode,
                SignatureType = SignatureType.HMAC
            };

            context.TrustedTouchpoints.Add(touchpoint);
            await context.SaveChangesAsync();
            return touchpoint;
        }

        // AccessPolicy
        private static async Task EnsurePolicyAsync(
            LyraDbContext context,
            Guid callerId,
            Guid targetId,
            string operation,
            AccessContext contextType)
        {
            bool exists = await context.AccessPolicies.AnyAsync(p =>
                p.CallerId == callerId &&
                p.TargetId == targetId &&
                p.Operation == operation &&
                p.Context == contextType);

            if (!exists)
            {
                var policy = new AccessPolicyEntity
                {
                    Id = Guid.NewGuid(),
                    CallerId = callerId,
                    TargetId = targetId,
                    Operation = operation,
                    Context = contextType,
                    IsEnabled = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.AccessPolicies.Add(policy);
                await context.SaveChangesAsync();
            }
        }
    }
}
