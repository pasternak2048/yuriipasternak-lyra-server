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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Company = await _companyService.GetByIdAsync(id);
            if (Company == null)
                return NotFound();

            Touchpoints = await _touchpointService.GetByCompanyAsync(id);
            return Page();
        }
    }
}
