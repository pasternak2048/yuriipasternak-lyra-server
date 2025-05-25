using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedAgentService _trustedAgentService;

        public IndexModel(ICompanyService companyService, ITrustedAgentService trustedAgentService)
        {
            _companyService = companyService;
            _trustedAgentService = trustedAgentService;
        }

        public int CompanyCount { get; set; }
        public int AgentCount { get; set; }

        public async Task OnGetAsync()
        {
            CompanyCount = await _companyService.GetTotalCompanyCountAsync();
            AgentCount = await _trustedAgentService.GetTotalAgentCountAsync();
        }
    }
}
