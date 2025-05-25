using LYRA.Server.Models.Agents;
using LYRA.Server.Models.Companies;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedAgents
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITrustedAgentService _agentService;
        private readonly ICompanyService _companyService;

        public IndexModel(ITrustedAgentService agentService, ICompanyService companyService)
        {
            _agentService = agentService;
            _companyService = companyService;
        }

        public PaginatedResult<TrustedAgentDto> Agents { get; set; } = new();
        public List<CompanyDto> Companies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public TrustedAgentFilters Filters { get; set; } = new();

        public async Task OnGetAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            Agents = await _agentService.GetPagedAsync(Filters);
        }
    }
}
