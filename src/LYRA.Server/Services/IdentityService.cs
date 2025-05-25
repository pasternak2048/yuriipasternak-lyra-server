using LYRA.Server.Entities.Identity;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LYRA.Server.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            return result.Succeeded
                ? (true, null)
                : (false, "Invalid credentials");

        }

        public async Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
                return (false, "Passwords do not match");

            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            await _signInManager.SignInAsync(user, isPersistent: false);
            return (true, null);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
