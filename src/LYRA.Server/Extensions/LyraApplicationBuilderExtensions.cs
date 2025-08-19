namespace LYRA.Server.Extensions
{
	public static class LyraApplicationBuilderExtensions
	{
		public static IServiceCollection AddLyraApplication(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddLyraDbContexts(configuration);
			services.AddLyraMilanoCache(configuration);
			services.AddLyraIdentity();
			services.AddLyraAuth();
			services.AddLyraServices();
			services.AddLyraWebApi();
			services.AddLyraRealTime();
			services.AddRazorPages();

			return services;
		}
	}
}
