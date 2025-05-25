using LYRA.Server.Models.Agents;
using LYRA.Server.Models.Companies;
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
        private readonly ITrustedAgentService _agentService;

        public DetailsModel(ICompanyService companyService, ITrustedAgentService agentService)
        {
            _companyService = companyService;
            _agentService = agentService;
        }

        public CompanyDto? Company { get; set; }
        public List<TrustedAgentDto> Agents { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Company = await _companyService.GetByIdAsync(id);
            if (Company == null) return NotFound();

            Agents = await _agentService.GetByCompanyAsync(id);
            return Page();
        }
    }
}
