using LYRA.Security.Enums;
using LYRA.Server.Models.Company;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Company.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    /// <summary>
    /// Razor Page model for creating a new Trusted Touchpoint.
    /// </summary>
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _touchpointService;

        public CreateModel(ICompanyService companyService, ITrustedTouchpointService touchpointService)
        {
            _companyService = companyService;
            _touchpointService = touchpointService;
        }

        /// <summary>
        /// The input form data for creating a new Trusted Touchpoint.
        /// </summary>
        [BindProperty]
        public TrustedTouchpointCreateRequest Input { get; set; } = new();

        /// <summary>
        /// List of available companies to assign the touchpoint to.
        /// </summary>
        public List<CompanyDto> Companies { get; set; } = new();

        /// <summary>
        /// List of selectable signature types (HMAC, RSA, None).
        /// </summary>
        public List<SelectListItem> SignatureTypes { get; set; } = new();

        // ---------------- TempData for confirmation page ---------------- //

        /// <summary>
        /// One-time shown plaintext secret.
        /// </summary>
        [TempData]
        public string? SecretPlaintext { get; set; }

        /// <summary>
        /// Display name of the created touchpoint.
        /// </summary>
        [TempData]
        public string? DisplayName { get; set; }

        /// <summary>
        /// System name of the created touchpoint.
        /// </summary>
        [TempData]
        public string? SystemName { get; set; }

        /// <summary>
        /// Name of the company associated with the touchpoint.
        /// </summary>
        [TempData]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Touchpoint active status.
        /// </summary>
        [TempData]
        public string? IsActive { get; set; }

        /// <summary>
        /// Whether company secret is used.
        /// </summary>
        [TempData]
        public string? UseCompanySecret { get; set; }

        /// <summary>
        /// Role mode of the touchpoint (CallerOnly, TargetOnly, Both).
        /// </summary>
        [TempData]
        public string? Mode { get; set; }

        /// <summary>
        /// Signature type used (HMAC, RSA, None).
        /// </summary>
        [TempData]
        public string? SignatureType { get; set; }

        /// <summary>
        /// Creation timestamp of the touchpoint.
        /// </summary>
        [TempData]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// ID of the created touchpoint.
        /// </summary>
        [TempData]
        public Guid Id { get; set; }

        /// <summary>
        /// Feedback message to display after creation.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Handles GET requests. Loads companies and signature types for the form.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            SignatureTypes = EnumHelper.GetSelectList<SignatureType>();
            return Page();
        }

        /// <summary>
        /// Handles POST requests. Attempts to create a new Trusted Touchpoint and redirect to the secret page.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            SignatureTypes = EnumHelper.GetSelectList<SignatureType>();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var created = await _touchpointService.AddAsync(Input);

                SecretPlaintext = created.SecretPlaintext;
                DisplayName = created.DisplayName;
                SystemName = created.SystemName;
                CompanyName = created.CompanyName;
                IsActive = created.IsActive.ToString();
                UseCompanySecret = created.UseCompanySecret.ToString();
                Mode = created.Mode;
                SignatureType = created.SignatureType;
                CreatedAt = created.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                Id = created.Id;

                Message = $"Trusted Touchpoint '{DisplayName}' created successfully.";
                return RedirectToPage("Secret", new { id = created.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
