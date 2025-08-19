using LYRA.Server.Data.LyraCachedDb;
using LYRA.Server.Data.LyraDb;
using LYRA.Server.Data.LyraLogsDb;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Extensions
{
	public static class AppBuilderExtensions
	{
		public static async Task MigrateAndSeedAsync(this IHost app)
		{
			using var scope = app.Services.CreateScope();

			var lyraDbContext = scope.ServiceProvider.GetRequiredService<LyraDbContext>();
			var lyraCachedDbContext = scope.ServiceProvider.GetRequiredService<LyraCachedDbContext>();
			var lyraLogsDbContext = scope.ServiceProvider.GetRequiredService<LyraLogsDbContext>();

			await lyraDbContext.Database.MigrateAsync();
			await lyraCachedDbContext.Database.MigrateAsync();
			await lyraLogsDbContext.Database.MigrateAsync();
			await DbInitializer.SeedAsync(scope.ServiceProvider);
		}
	}
}
