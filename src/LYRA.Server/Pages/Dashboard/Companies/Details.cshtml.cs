using LYRA.Server.Models.Company;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Company.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    /// <summary>
    /// Razor Page model for displaying details of a specific company,
    /// along with its trusted touchpoints, and supporting secret rotation and deletion.
    /// </summary>
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _touchpointService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailsModel"/> class.
        /// </summary>
        public DetailsModel(ICompanyService companyService, ITrustedTouchpointService touchpointService)
        {
            _companyService = companyService;
            _touchpointService = touchpointService;
        }

        /// <summary>
        /// Company information to display.
        /// </summary>
        public CompanyDto? Company { get; set; }

        /// <summary>
        /// List of associated trusted touchpoints for the company.
        /// </summary>
        public List<TrustedTouchpointDto> Touchpoints { get; set; } = new();

        /// <summary>
        /// Newly generated secret (shown once).
        /// </summary>
        [TempData]
        public string? SecretPlaintext { get; set; }

        [TempData] public string? DisplayName { get; set; }
        [TempData] public string? SystemName { get; set; }
        [TempData] public string? IsActive { get; set; }
        [TempData] public string? CreatedAt { get; set; }

        /// <summary>
        /// Company ID (for reloading or linking).
        /// </summary>
        [TempData]
        public Guid Id { get; set; }

        /// <summary>
        /// Optional status message.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Loads the company details and its trusted touchpoints.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Company = await _companyService.GetByIdAsync(id);
            if (Company == null) return NotFound();

            Touchpoints = await _touchpointService.GetByCompanyAsync(id);
            return Page();
        }

        /// <summary>
        /// Handles secret rotation for the given company and redirects to Secret page.
        /// </summary>
        public async Task<IActionResult> OnPostRotateAsync(Guid id)
        {
            try
            {
                var result = await _companyService.RotateSecretAsync(id);
                if (result == null) return NotFound();

                var company = await _companyService.GetByIdAsync(id);
                if (company == null) return NotFound();

                SecretPlaintext = result.SecretPlaintext;
                DisplayName = company.DisplayName;
                SystemName = company.SystemName;
                IsActive = company.IsActive.ToString();
                CreatedAt = company.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                Id = company.Id;

                Message = $"Secret rotated for company '{DisplayName}'.";
                return RedirectToPage("Secret", new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return await OnGetAsync(id);
            }
        }

        /// <summary>
        /// Handles deletion of the company and redirects to index.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                await _companyService.DeleteAsync(id);
                Message = $"Company deleted successfully.";
                return RedirectToPage("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return await OnGetAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
