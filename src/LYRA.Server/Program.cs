using LYRA.Server.Data.Core.Auditing;
using LYRA.Server.Data.Core.Caching;
using LYRA.Server.Data.LyraCachedDb;
using LYRA.Server.Data.LyraDb;
using LYRA.Server.Data.LyraLogsDb;
using LYRA.Server.Entities.Identity;
using LYRA.Server.Hubs;
using LYRA.Server.Services.AccessPolicy;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.AccessPolicy.Stores;
using LYRA.Server.Services.Company;
using LYRA.Server.Services.Company.Interfaces;
using LYRA.Server.Services.Identity;
using LYRA.Server.Services.Identity.Interfaces;
using LYRA.Server.Services.Logging;
using LYRA.Server.Services.Logging.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using LYRA.Server.Services.Verify;
using LYRA.Server.Services.Verify.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MILANO.Client.Extensions;
using MILANO.Client.Interfaces;
using System.Text.Json.Serialization;

/// <summary>
/// Entry point for the LYRA.Server application.
/// Configures services, middleware, authentication, and database migration for Razor Pages with security verification.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Register SignalR for real-time communication over WebSockets.
/// Enables broadcasting live events (e.g. activity logs) to connected clients via hubs.
/// </summary>
builder.Services.AddSignalR();

/// <summary>
/// Configure the database context with an auditing interceptor and SQL Server as the underlying provider.
/// </summary>
builder.Services.AddScoped<AuditableEntitySaveChangesInterceptor>();
builder.Services.AddScoped<AccessPolicyCacheSyncInterceptor>();

builder.Services.AddDbContext<LyraDbContext>((provider, options) =>
{
    var audit = provider.GetRequiredService<AuditableEntitySaveChangesInterceptor>();
    var cacheSync = provider.GetRequiredService<AccessPolicyCacheSyncInterceptor>();

    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
           .AddInterceptors(audit, cacheSync);
});

builder.Services.AddDbContext<LyraCachedDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CachedDatabase"));
});

builder.Services.AddDbContext<LyraLogsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LogsDatabase"));
});

/// <summary>
/// Register application services for current user tracking and entity auditing.
/// </summary>
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditableEntitySaveChangesInterceptor>();

/// <summary>
/// Configure ASP.NET Core Identity with Entity Framework and basic password policy.
/// </summary>
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LyraDbContext>()
    .AddDefaultTokenProviders();

/// <summary>
/// Register domain services for company, touchpoint, access policy, and identity management.
/// </summary>
builder.Services.AddMilanoCacheClient(options =>
{
	options.ServerHost = "http://host.docker.internal:7010";
	options.ApiKey = "test-key-full-access";
	options.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ITrustedTouchpointService, TrustedTouchpointService>();
builder.Services.AddScoped<IAccessPolicyService, AccessPolicyService>();
builder.Services.AddScoped<IVerifyService, VerifyService>();
builder.Services.AddScoped<ICachedAccessPolicyBuilder, CachedAccessPolicyBuilder>();
builder.Services.AddSingleton<IAccessPolicyCacheKeyBuilder, AccessPolicyCacheKeyBuilder>();
builder.Services.AddScoped<CachedAccessPolicyStore>();
builder.Services.AddScoped<ICachedAccessPolicyStore>(provider =>
{
	var store = provider.GetRequiredService<CachedAccessPolicyStore>();
	var cache = provider.GetRequiredService<IMilanoCacheClient>();
	var keyBuilder = provider.GetRequiredService<IAccessPolicyCacheKeyBuilder>();

	return new CachedAccessPolicyStoreDecorator(store, cache, keyBuilder);
});
builder.Services.AddScoped<IAccessPolicyCacheKeyBuilder, AccessPolicyCacheKeyBuilder>();
builder.Services.AddScoped<IAccessPolicyCacheSyncService, AccessPolicyCacheSyncService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddSingleton<ILogQueue, InMemoryLogQueue>();
builder.Services.AddHostedService<BackgroundLogWriterService>();

/// <summary>
/// Add Web API support for attribute-based controllers (e.g., VerificationController).
/// </summary>
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); ;

/// <summary>
/// Add Razor Pages framework.
/// </summary>
builder.Services.AddRazorPages();

/// <summary>
/// Configure password policy for development simplicity (can be hardened in production).
/// </summary>
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

/// <summary>
/// Configure application cookie settings for login redirection and security.
/// </summary>
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.Cookie.HttpOnly = true;
});

/// <summary>
/// Add authentication and authorization services.
/// </summary>
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorization();

/// <summary>
/// Provides access to the current HTTP context (required for ICurrentUserService).
/// </summary>
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

/// <summary>
/// Automatically apply migrations and seed initial data during startup.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var lyraDbContext = scope.ServiceProvider.GetRequiredService<LyraDbContext>();
    var lyraCachedDbContext = scope.ServiceProvider.GetRequiredService<LyraCachedDbContext>();
    var lyraLogsDbContext = scope.ServiceProvider.GetRequiredService<LyraLogsDbContext>();
    await lyraDbContext.Database.MigrateAsync();
    await lyraCachedDbContext.Database.MigrateAsync();
    await lyraLogsDbContext.Database.MigrateAsync();
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

/// <summary>
/// Configure exception handling and security for non-development environments.
/// </summary>
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

/// <summary>
/// Maps API controllers to their routes (e.g., /api/verification).
/// </summary>
app.MapControllers();

app.MapHub<LyraActivityHub>("/activityHub");

app.Run();