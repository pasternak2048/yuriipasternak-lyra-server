using LYRA.Server.Data.Core.Auditing;
using LYRA.Server.Data.Core.Caching;
using LYRA.Server.Data.LyraCachedDb;
using LYRA.Server.Data.LyraDb;
using LYRA.Server.Data.LyraLogsDb;
using LYRA.Server.Entities.Identity;
using LYRA.Server.Services.AccessPolicy;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.AccessPolicy.Stores;
using LYRA.Server.Services.Company;
using LYRA.Server.Services.Company.Interfaces;
using LYRA.Server.Services.Identity;
using LYRA.Server.Services.Identity.Interfaces;
using LYRA.Server.Services.Logging;
using LYRA.Server.Services.Logging.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using LYRA.Server.Services.Verify;
using LYRA.Server.Services.Verify.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MILANO.Client.Configuration;
using MILANO.Client.Extensions;
using MILANO.Client.Interfaces;
using System.Text.Json.Serialization;

namespace LYRA.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLyraCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ITrustedTouchpointService, TrustedTouchpointService>();
            services.AddScoped<IAccessPolicyService, AccessPolicyService>();
            services.AddScoped<IVerifyService, VerifyService>();
            services.AddScoped<IReplayProtectionStore, ReplayProtectionStore>();

            services.AddScoped<ICachedAccessPolicyBuilder, CachedAccessPolicyBuilder>();
            services.AddSingleton<IAccessPolicyCacheKeyBuilder, AccessPolicyCacheKeyBuilder>();
            services.AddScoped<CachedAccessPolicyStore>();
            services.AddScoped<ICachedAccessPolicyStore>(provider =>
            {
                var store = provider.GetRequiredService<CachedAccessPolicyStore>();
                var cache = provider.GetRequiredService<IMilanoCacheClient>();
                var keyBuilder = provider.GetRequiredService<IAccessPolicyCacheKeyBuilder>();
                return new CachedAccessPolicyStoreDecorator(store, cache, keyBuilder);
            });
            services.AddScoped<IAccessPolicyCacheSyncService, AccessPolicyCacheSyncService>();
            services.AddScoped<ILogService, LogService>();
            services.AddSingleton<ILogQueue, InMemoryLogQueue>();
            services.AddHostedService<BackgroundLogWriterService>();

            return services;
        }

        public static IServiceCollection AddLyraAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(IdentityConstants.ApplicationScheme);
            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddLyraIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<LyraDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.Cookie.HttpOnly = true;
            });

            return services;
        }

        public static IServiceCollection AddLyraDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<AccessPolicyCacheSyncInterceptor>();

            services.AddDbContext<LyraDbContext>((provider, options) =>
            {
                var audit = provider.GetRequiredService<AuditableEntitySaveChangesInterceptor>();
                var cacheSync = provider.GetRequiredService<AccessPolicyCacheSyncInterceptor>();
                options.UseSqlServer(configuration.GetConnectionString("Database"))
                       .AddInterceptors(audit, cacheSync);
            });

            services.AddDbContext<LyraCachedDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CachedDatabase"));
            });

            services.AddDbContext<LyraLogsDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("LogsDatabase"));
            });

            return services;
        }

        public static IServiceCollection AddLyraMilanoCache(this IServiceCollection services, IConfiguration config)
        {
            var options = config.GetSection("MilanoClient").Get<MilanoClientOptions>();

            services.AddMilanoCacheClient(opt =>
            {
                opt.ServerHost = options.ServerHost;
                opt.ApiKey = options.ApiKey;
                opt.Timeout = options.Timeout;
            });

            return services;
        }

        public static IServiceCollection AddLyraRealTime(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }

        public static IServiceCollection AddLyraWebApi(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            return services;
        }
    }
}
