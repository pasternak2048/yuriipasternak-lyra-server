using LYRA.Server.Data.LyraDb;
using LYRA.Server.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace LYRA.Server.Extensions
{
	public static class IdentityExtensions
	{
		public static IServiceCollection AddLyraIdentity(this IServiceCollection services)
		{
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<LyraDbContext>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 4;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
			});

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/account/login";
				options.Cookie.HttpOnly = true;
			});

			return services;
		}
	}
}
