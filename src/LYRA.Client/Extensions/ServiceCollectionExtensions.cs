using LYRA.Client.Abstractions;
using LYRA.Client.Configuration;
using LYRA.Client.Core;
using LYRA.Client.Touchpoints;
using Microsoft.Extensions.DependencyInjection;

namespace LYRA.Client.Extensions
{
	/// <summary>
	/// Provides extension methods to register LYRA client services into the DI container.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers LYRA for outbound signing (caller).
		/// </summary>
		public static IServiceCollection AddLyraSigningClient(
			this IServiceCollection services,
			Action<LyraSigningOptions> configure)
		{
			var options = new LyraSigningOptions();
			configure(options);

			services.AddSingleton<ITouchpointResolver>(new InMemoryTouchpointResolver(options.Touchpoints));
			services.AddSingleton<SigningService>();
			services.AddSingleton<ILyraClient, LyraClient>();

			return services;
		}

		/// <summary>
		/// Registers LYRA for inbound verification (receiver) over HTTP.
		/// </summary>
		/// <param name="services">DI container</param>
		/// <param name="serverBaseUrl">Base URL of LYRA.Server (e.g., https://lyra.company.com/)</param>
		/// <param name="verifyPath">Relative verify path (default: "api/verify")</param>
		public static IServiceCollection AddLyraVerificationClient(
			this IServiceCollection services,
			string serverBaseUrl,
			string verifyPath = "api/verify")
		{
			services.AddHttpClient<VerifyService>(client =>
			{
				client.BaseAddress = new Uri(serverBaseUrl, UriKind.Absolute);
			});

			services.AddSingleton<ILyraClient, LyraClient>();

			return services;
		}
	}
}
