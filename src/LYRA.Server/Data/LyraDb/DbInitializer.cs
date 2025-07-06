using LYRA.Security.Enums;
using LYRA.Security.Utilities;
using LYRA.Security.Utilities.Security;
using LYRA.Server.Entities;
using LYRA.Server.Entities.Identity;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.Logging.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Data.LyraDb
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<LyraDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var cacheSync = serviceProvider.GetRequiredService<IAccessPolicyCacheSyncService>();
            var logger = serviceProvider.GetRequiredService<ILogService>();

            context.Database.EnsureCreated();
            await logger.WriteAsync("System", "Info", "Database ensured created", source: "DbInitializer");

            if (await context.Companies.AnyAsync())
            {
                await logger.WriteAsync("System", "Info", "Seed skipped: companies already exist", source: "DbInitializer");
                return;
            }

            // Admin
            const string adminEmail = "admin@lyra";
            const string adminPassword = "admin";

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
                    var error = string.Join(", ", result.Errors.Select(e => e.Description));
                    await logger.WriteAsync("System", "Fail", "Failed to create admin user", source: "DbInitializer", exception: error);
                    throw new Exception("Failed to create admin user: " + error);
                }

                await logger.WriteAsync("System", "Success", "Admin user created", source: "DbInitializer");
            }
            else
            {
                await logger.WriteAsync("System", "Info", "Admin user already exists", source: "DbInitializer");
            }

            var systemUserId = Guid.Parse(adminUser.Id);

            // Companies
            var companies = new List<CompanyEntity>();
            for (int i = 1; i <= 30; i++)
            {
                var displayName = $"Company #{i:00}";
                var secret = EncryptionHelper.EncryptSecret($"secret-{i:00}");

                var entity = new CompanyEntity
                {
                    Id = Guid.NewGuid(),
                    DisplayName = displayName,
                    SystemName = SlugHelper.Slugify(displayName),
                    Secret = secret,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = systemUserId
                };

                companies.Add(entity);
            }

            context.Companies.AddRange(companies);
            await context.SaveChangesAsync();
            await logger.WriteAsync("System", "Success", "30 Companies created", source: "DbInitializer");

            // Touchpoints
            var touchpoints = new List<TrustedTouchpointEntity>();
            for (int i = 0; i < companies.Count; i++)
            {
                var c = companies[i];
                var name = $"Default TP #{i + 1:00}";
                var tpSlug = SlugHelper.Slugify(name);
                var systemName = $"{tpSlug}@{c.SystemName}";
                var secret = EncryptionHelper.EncryptSecret($"tp-secret-{i + 1:00}");

                var tp = new TrustedTouchpointEntity
                {
                    Id = Guid.NewGuid(),
                    CompanyId = c.Id,
                    DisplayName = name,
                    SystemName = systemName,
                    Secret = secret,
                    UseCompanySecret = false,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = systemUserId,
                    Mode = TouchpointMode.Both,
                    SignatureType = SignatureType.HMAC
                };

                touchpoints.Add(tp);
            }

            context.TrustedTouchpoints.AddRange(touchpoints);
            await context.SaveChangesAsync();
            await logger.WriteAsync("System", "Success", "30 Touchpoints created", source: "DbInitializer");

            // Policies (cyclic: 0 -> 1, 1 -> 2, ..., 29 -> 0)
            var policies = new List<AccessPolicyEntity>();
            for (int i = 0; i < touchpoints.Count; i++)
            {
                var caller = touchpoints[i];
                var target = touchpoints[(i + 1) % touchpoints.Count]; // next one or wrap

                var policy = new AccessPolicyEntity
                {
                    Id = Guid.NewGuid(),
                    CallerId = caller.Id,
                    TargetId = target.Id,
                    CallerSystemName = caller.SystemName,
                    TargetSystemName = target.SystemName,
                    Operation = "POST /api/verify",
                    Context = AccessContext.Http,
                    IsEnabled = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = systemUserId
                };

                policies.Add(policy);
            }

            context.AccessPolicies.AddRange(policies);
            await context.SaveChangesAsync();
            await logger.WriteAsync("System", "Success", "30 Policies created", source: "DbInitializer");

            // Cache
            await cacheSync.SyncFromDbAsync();
            await logger.WriteAsync("System", "Success", "Access policy cache generated", source: "DbInitializer");
        }
    }
}
