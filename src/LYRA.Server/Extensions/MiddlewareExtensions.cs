using LYRA.Server.Hubs;

namespace LYRA.Server.Extensions
{
	public static class MiddlewareExtensions
	{
		public static async Task UseLyraMiddleware(this WebApplication app)
		{
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			//app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.MapRazorPages();
			app.MapHub<LyraActivityHub>("/activityHub");

			await app.MigrateAndSeedAsync();
		}
	}
}
