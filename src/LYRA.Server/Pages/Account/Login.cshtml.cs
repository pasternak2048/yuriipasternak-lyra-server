using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Pages.Account
{
    /// <summary>
    /// Razor Page model for user login functionality.
    /// Handles user input, validation, and authentication via IIdentityService.
    /// </summary>
    public class LoginModel : PageModel
    {
        private readonly IIdentityService _identityService;

        public LoginModel(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// User-provided credentials for login (email and password).
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Optional return URL after successful login (set via query string).
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Error message to display on the login page (if any).
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Represents the user input model for login.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Email address used for login.
            /// </summary>
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Password associated with the email.
            /// </summary>
            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        /// <summary>
        /// Handles initial GET request to ensure ReturnUrl is set.
        /// </summary>
        public void OnGet()
        {
            ReturnUrl ??= Url.Content("~/");
        }

        /// <summary>
        /// Handles POST request for login.
        /// Validates user credentials and redirects or returns page with errors.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            ReturnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _identityService.LoginAsync(Input.Email, Input.Password);
            if (!result.Success)
            {
                ErrorMessage = result.Error;
                return Page();
            }

            return LocalRedirect(ReturnUrl);
        }
    }
}
