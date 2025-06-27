using LYRA.Security.Signature;
using LYRA.Server.Data.Core.Auditing;
using LYRA.Server.Data.Core.Caching;
using LYRA.Server.Data.LyraCachedDb;
using LYRA.Server.Data.LyraDb;
using LYRA.Server.Entities.Identity;
using LYRA.Server.Services;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Services.SecurityVerification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

/// <summary>
/// Entry point for the LYRA.Server application.
/// Configures services, middleware, authentication, and database migration for Razor Pages with security verification.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ITrustedTouchpointService, TrustedTouchpointService>();
builder.Services.AddScoped<IAccessPolicyService, AccessPolicyService>();
builder.Services.AddScoped<IVerifyService, VerifyService>();
builder.Services.AddScoped<ICachedAccessPolicyBuilder, CachedAccessPolicyBuilder>();
builder.Services.AddScoped<ICachedAccessPolicyService, CachedAccessPolicyService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<ICachedAccessPolicyMemoryService, CachedAccessPolicyMemoryService>();
builder.Services.AddScoped<IAccessPolicyCacheSyncService, AccessPolicyCacheSyncService>();

/// <summary>
/// Register signature string builders per access context and the factory to resolve them.
/// </summary>
builder.Services.AddTransient<ISignatureStringBuilder, HttpSignatureStringBuilder>();
builder.Services.AddTransient<ISignatureStringBuilder, CacheSignatureStringBuilder>();
builder.Services.AddSingleton<SignatureStringBuilderFactory>();

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
    await lyraDbContext.Database.MigrateAsync();
    await lyraCachedDbContext.Database.MigrateAsync();
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

app.Run();