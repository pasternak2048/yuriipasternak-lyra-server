using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ICompanyService _companyService;

        public IndexModel(ITrustedTouchpointService touchpointService, ICompanyService companyService)
        {
            _touchpointService = touchpointService;
            _companyService = companyService;
        }

        /// <summary>
        /// List of trusted touchpoints matching the current filter
        /// </summary>
        public PaginatedResult<TrustedTouchpointDto> Touchpoints { get; set; } = new();

        /// <summary>
        /// All companies (for filtering dropdown)
        /// </summary>
        public List<CompanyDto> Companies { get; set; } = new();

        /// <summary>
        /// Current filter and pagination state
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public TrustedTouchpointFilters Filters { get; set; } = new();

        public async Task OnGetAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            Touchpoints = await _touchpointService.GetPagedAsync(Filters);
        }
    }
}
