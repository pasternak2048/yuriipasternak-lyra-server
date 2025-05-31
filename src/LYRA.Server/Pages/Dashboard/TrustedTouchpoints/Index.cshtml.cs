using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    /// <summary>
    /// Razor Page model for listing Trusted Touchpoints with optional filtering.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ICompanyService _companyService;

        /// <summary>
        /// Constructor with required services.
        /// </summary>
        public IndexModel(ITrustedTouchpointService touchpointService, ICompanyService companyService)
        {
            _touchpointService = touchpointService;
            _companyService = companyService;
        }

        /// <summary>
        /// List of trusted touchpoints matching the current filter.
        /// </summary>
        public PaginatedResult<TrustedTouchpointDto> Touchpoints { get; set; } = new();

        /// <summary>
        /// List of all companies for use in filter dropdown.
        /// </summary>
        public List<CompanyDto> Companies { get; set; } = new();

        /// <summary>
        /// Current filtering and pagination state for the page.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public TrustedTouchpointFilters Filters { get; set; } = new();

        /// <summary>
        /// Handles initial page load and populates data.
        /// </summary>
        public async Task OnGetAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            Touchpoints = await _touchpointService.GetPagedAsync(Filters);
        }
    }
}
