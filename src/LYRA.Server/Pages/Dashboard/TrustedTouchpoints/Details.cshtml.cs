using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    /// <summary>
    /// Razor Page model for viewing and managing details of a Trusted Touchpoint.
    /// </summary>
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ITrustedTouchpointService _touchpointService;

        public DetailsModel(ITrustedTouchpointService touchpointService)
        {
            _touchpointService = touchpointService;
        }

        /// <summary>
        /// Trusted Touchpoint details loaded for the page.
        /// </summary>
        public TrustedTouchpointDto? Touchpoint { get; set; }

        // ------------- TempData for Secret/Confirmation Page -------------

        /// <summary> One-time shown plaintext secret. </summary>
        [TempData] public string? SecretPlaintext { get; set; }

        /// <summary> Display name of the touchpoint. </summary>
        [TempData] public string? DisplayName { get; set; }

        /// <summary> System name of the touchpoint. </summary>
        [TempData] public string? SystemName { get; set; }

        /// <summary> Company name owning the touchpoint. </summary>
        [TempData] public string? CompanyName { get; set; }

        /// <summary> Touchpoint active status. </summary>
        [TempData] public string? IsActive { get; set; }

        /// <summary> Whether the company-wide secret is used. </summary>
        [TempData] public string? UseCompanySecret { get; set; }

        /// <summary> Signature type used. </summary>
        [TempData] public string? SignatureType { get; set; }

        /// <summary> Creation timestamp. </summary>
        [TempData] public string? CreatedAt { get; set; }

        /// <summary> ID of the touchpoint. </summary>
        [TempData] public Guid Id { get; set; }

        /// <summary> Temporary message for confirmation. </summary>
        [TempData] public string? Message { get; set; }

        /// <summary>
        /// Handles GET requests to load touchpoint details.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Touchpoint = await _touchpointService.GetByIdAsync(id);
            if (Touchpoint == null)
                return NotFound();

            return Page();
        }

        /// <summary>
        /// Handles POST request to rotate the secret for the touchpoint.
        /// Stores new secret in TempData and redirects to secret page.
        /// </summary>
        public async Task<IActionResult> OnPostRotateAsync(Guid id)
        {
            try
            {
                var result = await _touchpointService.RotateSecretAsync(id);
                if (result == null)
                    return NotFound();

                var touchpoint = await _touchpointService.GetByIdAsync(id);
                if (touchpoint == null)
                    return NotFound();

                SecretPlaintext = result.SecretPlaintext;
                DisplayName = touchpoint.DisplayName;
                SystemName = touchpoint.SystemName;
                CompanyName = touchpoint.CompanyName;
                IsActive = touchpoint.IsActive.ToString();
                UseCompanySecret = touchpoint.UseCompanySecret.ToString();
                SignatureType = touchpoint.SignatureType.ToString();
                CreatedAt = touchpoint.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                Id = touchpoint.Id;

                return RedirectToPage("Secret", new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Touchpoint = await _touchpointService.GetByIdAsync(id);
                return Page();
            }
        }

        /// <summary>
        /// Handles POST request to delete the touchpoint.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                await _touchpointService.DeleteAsync(id);
                Message = "Trusted Touchpoint deleted successfully.";
                return RedirectToPage("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Touchpoint = await _touchpointService.GetByIdAsync(id);
                return Page();
            }
        }
    }
}
