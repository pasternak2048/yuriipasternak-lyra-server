using LYRA.Server.Data.Core.Auditing;
using LYRA.Server.Data.Core.Caching;
using LYRA.Server.Data.LyraCachedDb;
using LYRA.Server.Data.LyraDb;
using LYRA.Server.Data.LyraLogsDb;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Extensions
{
	public static class DatabaseExtensions
	{
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
	}
}
