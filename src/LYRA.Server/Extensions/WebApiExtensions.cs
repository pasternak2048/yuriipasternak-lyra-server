using System.Text.Json.Serialization;

namespace LYRA.Server.Extensions
{
	public static class WebApiExtensions
	{
		public static IServiceCollection AddLyraWebApi(this IServiceCollection services)
		{
			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});
			return services;
		}
	}
}
