using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    /// <summary>
    /// Razor Page model for displaying the newly generated secret of a Trusted Touchpoint.
    /// This page is only shown once immediately after creation or rotation.
    /// </summary>
    [Authorize]
    public class SecretModel : PageModel
    {
        private readonly ITrustedTouchpointService _service;

        /// <summary>
        /// Initializes the model with required services.
        /// </summary>
        public SecretModel(ITrustedTouchpointService service)
        {
            _service = service;
        }

        /// <summary>
        /// The plain text version of the secret (only shown once).
        /// </summary>
        [TempData]
        public string? SecretPlaintext { get; set; }

        /// <summary>
        /// Display name of the touchpoint.
        /// </summary>
        [TempData]
        public string? DisplayName { get; set; }

        /// <summary>
        /// System name of the touchpoint.
        /// </summary>
        [TempData]
        public string? SystemName { get; set; }

        /// <summary>
        /// Name of the company that owns the touchpoint.
        /// </summary>
        [TempData]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Whether the touchpoint is active.
        /// </summary>
        [TempData]
        public string? IsActive { get; set; }

        /// <summary>
        /// Whether the touchpoint uses the company-wide secret.
        /// </summary>
        [TempData]
        public string? UseCompanySecret { get; set; }

        /// <summary>
        /// Touchpoint mode: CallerOnly, TargetOnly, or Both.
        /// </summary>
        [TempData]
        public string? Mode { get; set; }

        /// <summary>
        /// Signature type: HMAC, RSA, or None.
        /// </summary>
        [TempData]
        public string? SignatureType { get; set; }

        /// <summary>
        /// Creation timestamp of the touchpoint.
        /// </summary>
        [TempData]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// Unique identifier of the touchpoint.
        /// </summary>
        [TempData]
        public Guid Id { get; set; }

        /// <summary>
        /// Optional success message.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Indicates whether the touchpoint is active.
        /// </summary>
        public bool IsActiveBool => IsActive == bool.TrueString;

        /// <summary>
        /// Indicates whether the touchpoint uses the company-wide secret.
        /// </summary>
        public bool UseCompanySecretBool => UseCompanySecret == bool.TrueString;

        /// <summary>
        /// Handles GET requests. Redirects if no secret is available (e.g., page accessed directly).
        /// </summary>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (string.IsNullOrWhiteSpace(SecretPlaintext))
                return RedirectToPage("Details", new { id });

            return Page();
        }
    }
}
