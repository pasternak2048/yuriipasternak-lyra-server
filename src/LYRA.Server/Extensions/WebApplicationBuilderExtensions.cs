namespace LYRA.Server.Extensions
{
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder AddLyraApplication(this WebApplicationBuilder builder)
		{
			var configuration = builder.Configuration;
			var services = builder.Services;

			services.AddLyraDbContexts(configuration);
			services.AddLyraMilanoCache(configuration);
			services.AddLyraIdentity();
			services.AddLyraAuthentication();
			services.AddLyraCoreServices();
			services.AddLyraWebApi();
			services.AddLyraRealTime();
			services.AddRazorPages();

			return builder;
		}
	}
}
