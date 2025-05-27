using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    [Authorize]
    public class SecretModel : PageModel
    {
        private readonly ITrustedTouchpointService _service;

        public SecretModel(ITrustedTouchpointService service)
        {
            _service = service;
        }

        [TempData] public string? SecretPlaintext { get; set; }

        [TempData] public string? DisplayName { get; set; }

        [TempData] public string? Name { get; set; }

        [TempData] public string? CompanyName { get; set; }

        [TempData] public string? IsActive { get; set; }

        [TempData] public string? UseCompanySecret { get; set; }

        [TempData] public string? Mode { get; set; }

        [TempData] public string? SignatureType { get; set; }

        [TempData] public string? CreatedAt { get; set; }

        [TempData] public Guid Id { get; set; }

        public bool IsActiveBool => IsActive == bool.TrueString;
        public bool UseCompanySecretBool => UseCompanySecret == bool.TrueString;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (string.IsNullOrWhiteSpace(SecretPlaintext))
                return RedirectToPage("Details", new { id });

            return Page();
        }
    }
}
