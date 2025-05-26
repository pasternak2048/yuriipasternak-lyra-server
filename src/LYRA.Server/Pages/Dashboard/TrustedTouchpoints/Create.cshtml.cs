using LYRA.Server.Models.Company;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
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

        [BindProperty]
        public TrustedTouchpointCreateRequest Input { get; set; } = new();

        public List<CompanyDto> Companies { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Companies = await _companyService.GetLightweightAsync();

            if (!ModelState.IsValid)
                return Page();

            var normalizedName = SlugHelper.Slugify(Input.DisplayName);
            var exists = await _touchpointService.ExistsByCompanyAndNameAsync(Input.CompanyId, normalizedName);

            if (exists)
            {
                ModelState.AddModelError("Input.DisplayName", "A touchpoint with this name already exists in the selected company.");
                return Page();
            }

            await _touchpointService.AddAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
