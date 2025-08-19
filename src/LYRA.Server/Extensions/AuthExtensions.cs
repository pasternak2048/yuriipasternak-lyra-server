using Microsoft.AspNetCore.Identity;

namespace LYRA.Server.Extensions
{
	public static class AuthExtensions
	{
		public static IServiceCollection AddLyraAuth(this IServiceCollection services)
		{
			services.AddAuthentication(IdentityConstants.ApplicationScheme);
			services.AddAuthorization();
			return services;
		}
	}
}
