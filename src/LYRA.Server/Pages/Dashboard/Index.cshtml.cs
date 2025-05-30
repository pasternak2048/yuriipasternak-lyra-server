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
        private readonly IAccessPolicyService _accessPolicyService;

        public IndexModel(ICompanyService companyService, ITrustedTouchpointService trustedTouchpointService, IAccessPolicyService accessPolicyService)
        {
            _companyService = companyService;
            _trustedTouchpointService = trustedTouchpointService;
            _accessPolicyService = accessPolicyService;
        }

        public int CompanyCount { get; set; }

        public int TouchpointCount { get; set; }

        public int PolicyCount { get; set; }

        public async Task OnGetAsync()
        {
            CompanyCount = await _companyService.GetTotalCompanyCountAsync();
            TouchpointCount = await _trustedTouchpointService.GetTotalTouchpointCountAsync();
            PolicyCount = await _accessPolicyService.GetTotalPolicyCountAsync();
        }
    }
}
