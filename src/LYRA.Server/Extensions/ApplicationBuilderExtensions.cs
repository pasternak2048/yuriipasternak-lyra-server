namespace LYRA.Server.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseLyraMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (!env.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			//app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			return app;
		}
	}
}
