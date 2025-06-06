using LYRA.Security.Enums;
using LYRA.Security.Utilities;
using LYRA.Security.Utilities.Security;
using LYRA.Server.Entities;
using LYRA.Server.Entities.Identity;
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

            if (await context.Companies.AnyAsync())
                return;

            const string adminEmail = "admin@lyra";
            const string adminPassword = "admin";

            // 1. Admin user
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
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

            var systemUserId = Guid.Parse(adminUser.Id);

            // 2. Companies
            var aCorp = await EnsureCompanyAsync(context, "A Corp", EncryptionHelper.EncryptSecret("a-secret"), systemUserId);
            var bCorp = await EnsureCompanyAsync(context, "B Corp", EncryptionHelper.EncryptSecret("b-secret"), systemUserId);
            var cCorp = await EnsureCompanyAsync(context, "C Corp", EncryptionHelper.EncryptSecret("c-secret"), systemUserId);

            // 3. Touchpoints
            var aBilling = await EnsureTouchpointAsync(context, aCorp, "Billing API", EncryptionHelper.EncryptSecret("a-billing-secret"), TouchpointMode.TargetOnly, systemUserId);
            var aPublicApi = await EnsureTouchpointAsync(context, aCorp, "Public API", EncryptionHelper.EncryptSecret("a-api-secret"), TouchpointMode.TargetOnly, systemUserId);

            var bGateway = await EnsureTouchpointAsync(context, bCorp, "Gateway", EncryptionHelper.EncryptSecret("b-gateway-secret"), TouchpointMode.CallerOnly, systemUserId);
            var bReport = await EnsureTouchpointAsync(context, bCorp, "Report Bot", EncryptionHelper.EncryptSecret("b-report-secret"), TouchpointMode.Both, systemUserId);

            var cWorker = await EnsureTouchpointAsync(context, cCorp, "Worker Node", EncryptionHelper.EncryptSecret("c-worker-secret"), TouchpointMode.CallerOnly, systemUserId);
            var cBot = await EnsureTouchpointAsync(context, cCorp, "Bot Commander", EncryptionHelper.EncryptSecret("c-bot-secret"), TouchpointMode.Both, systemUserId);

            // 4. Access policy
            await EnsurePolicyAsync(context, bGateway, aBilling, "POST /subscribe", AccessContext.Http, systemUserId);
        }

        private static async Task<CompanyEntity> EnsureCompanyAsync(
            LyraDbContext context,
            string displayName,
            string secret,
            Guid createdBy)
        {
            var slugName = SlugHelper.Slugify(displayName);

            var existing = await context.Companies.FirstOrDefaultAsync(c => c.SystemName == slugName);
            if (existing != null) return existing;

            var entity = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                SystemName = slugName,
                DisplayName = displayName,
                Secret = secret,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            context.Companies.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        private static async Task<TrustedTouchpointEntity> EnsureTouchpointAsync(
            LyraDbContext context,
            CompanyEntity company,
            string displayName,
            string secret,
            TouchpointMode mode,
            Guid createdBy)
        {
            var tpSlug = SlugHelper.Slugify(displayName);
            var fullName = $"{tpSlug}@{company.SystemName}";

            var existing = await context.TrustedTouchpoints
                .FirstOrDefaultAsync(t => t.SystemName == fullName);

            if (existing != null) return existing;

            var touchpoint = new TrustedTouchpointEntity
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                SystemName = fullName,
                DisplayName = displayName,
                Secret = secret,
                UseCompanySecret = false,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                Mode = mode,
                SignatureType = SignatureType.HMAC
            };

            context.TrustedTouchpoints.Add(touchpoint);
            await context.SaveChangesAsync();
            return touchpoint;
        }

        private static async Task EnsurePolicyAsync(
            LyraDbContext context,
            TrustedTouchpointEntity caller,
            TrustedTouchpointEntity target,
            string operation,
            AccessContext contextType,
            Guid createdBy)
        {
            var exists = await context.AccessPolicies.AnyAsync(p =>
                p.CallerSystemName == caller.SystemName &&
                p.TargetSystemName == target.SystemName &&
                p.Operation == operation &&
                p.Context == contextType);

            if (exists) return;

            var policy = new AccessPolicyEntity
            {
                Id = Guid.NewGuid(),
                CallerId = caller.Id,
                TargetId = target.Id,
                CallerSystemName = caller.SystemName,
                TargetSystemName = target.SystemName,
                Operation = operation,
                Context = contextType,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            context.AccessPolicies.Add(policy);
            await context.SaveChangesAsync();
        }
    }
}
