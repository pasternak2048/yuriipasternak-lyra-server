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
using MILANO.Client.Interfaces;

namespace LYRA.Server.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLyraServices(this IServiceCollection services)
		{
			services.AddScoped<ICurrentUserService, CurrentUserService>();

			services.AddScoped<IIdentityService, IdentityService>();
			services.AddScoped<ICompanyService, CompanyService>();
			services.AddScoped<ITrustedTouchpointService, TrustedTouchpointService>();
			services.AddScoped<IAccessPolicyService, AccessPolicyService>();
			services.AddScoped<IVerifyService, VerifyService>();
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
	}
}
