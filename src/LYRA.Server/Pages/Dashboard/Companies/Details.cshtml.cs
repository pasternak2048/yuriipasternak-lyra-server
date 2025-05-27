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

        [TempData] public string? SecretPlaintext { get; set; }

        [TempData] public string? DisplayName { get; set; }

        [TempData] public string? Name { get; set; }

        [TempData] public string? IsActive { get; set; }

        [TempData] public string? CreatedAt { get; set; }

        [TempData] public Guid Id { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Company = await _companyService.GetByIdAsync(id);
            if (Company == null) return NotFound();

            Touchpoints = await _touchpointService.GetByCompanyAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostRotateAsync(Guid id)
        {
            var result = await _companyService.RotateSecretAsync(id);
            if (result == null) return NotFound();

            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();

            SecretPlaintext = result.SecretPlaintext;
            DisplayName = company.DisplayName;
            Name = company.Name;
            IsActive = company.IsActive.ToString();
            CreatedAt = company.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            Id = company.Id;

            return RedirectToPage("Secret", new { id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _companyService.DeleteAsync(id);
            return RedirectToPage("Index");
        }
    }
}
