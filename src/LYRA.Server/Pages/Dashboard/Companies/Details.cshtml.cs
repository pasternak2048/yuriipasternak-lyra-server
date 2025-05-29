using LYRA.Server.Models.Company;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _touchpointService;

        public DetailsModel(ICompanyService companyService, ITrustedTouchpointService touchpointService)
        {
            _companyService = companyService;
            _touchpointService = touchpointService;
        }

        public CompanyDto? Company { get; set; }

        public List<TrustedTouchpointDto> Touchpoints { get; set; } = new();

        [TempData]
        public string? SecretPlaintext { get; set; }
        [TempData]
        public string? DisplayName { get; set; }
        [TempData]
        public string? SystemName { get; set; }
        [TempData]
        public string? IsActive { get; set; }
        [TempData]
        public string? CreatedAt { get; set; }
        [TempData]
        public Guid Id { get; set; }
        [TempData]
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Company = await _companyService.GetByIdAsync(id);
            if (Company == null) return NotFound();

            Touchpoints = await _touchpointService.GetByCompanyAsync(id);
            return Page();
        }

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
                return await OnGetAsync(id); // reload details page with error
            }
        }

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
                return await OnGetAsync(id); // reload details with error
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }

}
