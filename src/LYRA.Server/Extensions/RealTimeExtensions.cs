namespace LYRA.Server.Extensions
{
	public static class RealTimeExtensions
	{
		public static IServiceCollection AddLyraRealTime(this IServiceCollection services)
		{
			services.AddSignalR();
			return services;
		}
	}
}
