using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _trustedTouchpointService;

        public IndexModel(ICompanyService companyService, ITrustedTouchpointService trustedTouchpointService)
        {
            _companyService = companyService;
            _trustedTouchpointService = trustedTouchpointService;
        }

        public int CompanyCount { get; set; }
        public int TouchpointCount { get; set; }

        public async Task OnGetAsync()
        {
            CompanyCount = await _companyService.GetTotalCompanyCountAsync();
            TouchpointCount = await _trustedTouchpointService.GetTotalTouchpointCountAsync();
        }
    }
}
