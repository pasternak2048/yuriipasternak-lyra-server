using LYRA.Server.Data;
using LYRA.Server.Data.Auditing;
using LYRA.Server.Entities.Identity;
using LYRA.Server.Services;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Services.SecurityVerification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LyraDbContext>((provider, options) =>
{
    var interceptor = provider.GetRequiredService<AuditableEntitySaveChangesInterceptor>();
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(interceptor);
});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditableEntitySaveChangesInterceptor>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LyraDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ITrustedTouchpointService, TrustedTouchpointService>();
builder.Services.AddScoped<IAccessPolicyService, AccessPolicyService>();
builder.Services.AddScoped<ISecretProvider, SecretProvider>();
builder.Services.AddTransient<ISignatureStringBuilder, HttpSignatureStringBuilder>();
builder.Services.AddTransient<ISignatureStringBuilder, CacheSignatureStringBuilder>();
builder.Services.AddSingleton<SignatureStringBuilderFactory>();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.Cookie.HttpOnly = true;
});

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LyraDbContext>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
