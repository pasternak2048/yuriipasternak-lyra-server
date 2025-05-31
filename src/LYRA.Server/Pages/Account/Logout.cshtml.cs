using LYRA.Server.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Account
{
    /// <summary>
    /// Razor Page model responsible for user logout functionality.
    /// Handles sign-out logic and redirects back to the login page.
    /// </summary>
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutModel"/> class.
        /// </summary>
        /// <param name="signInManager">ASP.NET Core Identity sign-in manager.</param>
        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// Handles GET requests. Simply redirects to the login page.
        /// </summary>
        public IActionResult OnGet()
        {
            return RedirectToPage("/Account/Login");
        }

        /// <summary>
        /// Handles POST requests. Signs out the user and redirects to login.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }
    }
}
