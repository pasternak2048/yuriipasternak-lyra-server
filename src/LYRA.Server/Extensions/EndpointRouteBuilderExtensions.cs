using LYRA.Server.Hubs;

namespace LYRA.Server.Extensions
{
	public static class EndpointRouteBuilderExtensions
	{
		public static IEndpointRouteBuilder MapLyraEndpoints(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapControllers();
			endpoints.MapRazorPages();
			endpoints.MapHub<LyraActivityHub>("/activityHub");
			return endpoints;
		}
	}
}
