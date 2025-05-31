using LYRA.Server.Enums;
using LYRA.Server.Models.Company;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    /// <summary>
    /// Razor Page model for editing a Trusted Touchpoint.
    /// </summary>
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _touchpointService;

        public EditModel(ICompanyService companyService, ITrustedTouchpointService touchpointService)
        {
            _companyService = companyService;
            _touchpointService = touchpointService;
        }

        /// <summary>
        /// Form input for editing a Trusted Touchpoint.
        /// </summary>
        [BindProperty]
        public TrustedTouchpointUpdateRequest Input { get; set; } = new();

        /// <summary>
        /// List of companies available for selection (if needed).
        /// </summary>
        public List<CompanyDto> Companies { get; set; } = new();

        /// <summary>
        /// List of available signature types for dropdown.
        /// </summary>
        public List<SelectListItem> SignatureTypes { get; set; } = new();

        /// <summary>
        /// Optional message to show after update.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Loads data to populate the edit form.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var touchpoint = await _touchpointService.GetByIdAsync(id);
            if (touchpoint == null) return NotFound();

            Input = new TrustedTouchpointUpdateRequest
            {
                Id = touchpoint.Id,
                DisplayName = touchpoint.DisplayName,
                IsActive = touchpoint.IsActive,
                UseCompanySecret = touchpoint.UseCompanySecret,
                Mode = touchpoint.Mode.ToString(),
                SignatureType = touchpoint.SignatureType.ToString(),
                Description = touchpoint.Description,
                AllowedSourceIp = touchpoint.AllowedSourceIp
            };

            SignatureTypes = EnumHelper.GetSelectList<SignatureType>();
            return Page();
        }

        /// <summary>
        /// Handles the update of a Trusted Touchpoint after form submission.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            SignatureTypes = EnumHelper.GetSelectList<SignatureType>();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _touchpointService.UpdateAsync(Input);
                Message = $"Touchpoint '{Input.DisplayName}' updated successfully.";
                return RedirectToPage("Details", new { id = Input.Id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
