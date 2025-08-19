using MILANO.Client.Configuration;
using MILANO.Client.Extensions;

namespace LYRA.Server.Extensions
{
	public static class MilanoCacheExtensions
	{
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
	}
}
