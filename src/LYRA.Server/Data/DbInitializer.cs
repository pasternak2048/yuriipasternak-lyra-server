using LYRA.Server.Entities;
using Microsoft.AspNetCore.Identity;

namespace LYRA.Server.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<LyraDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            context.Database.EnsureCreated();

            const string adminEmail = "admin@lyra.com";
            const string adminPassword = "admin";

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

            if (!context.TrustedServices.Any())
            {
                context.TrustedServices.Add(new TrustedService
                {
                    CompanyId = "demo",
                    Name = "gateway",
                    Secret = "demo-gateway-secret",
                    UseCompanySecret = false
                });
            }

            if (!context.AccessPolicies.Any())
            {
                context.AccessPolicies.Add(new AccessPolicy
                {
                    CompanyId = "demo",
                    Caller = "gateway",
                    Target = "subscription",
                    Method = "POST",
                    PathPattern = "/subscribe"
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
