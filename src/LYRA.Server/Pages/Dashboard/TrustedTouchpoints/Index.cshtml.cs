using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Company.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    /// <summary>
    /// Razor Page model for listing and filtering Trusted Touchpoints.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ICompanyService _companyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class with required services.
        /// </summary>
        /// <param name="touchpointService">Service for managing trusted touchpoints.</param>
        /// <param name="companyService">Service for retrieving company information.</param>
        public IndexModel(ITrustedTouchpointService touchpointService, ICompanyService companyService)
        {
            _touchpointService = touchpointService;
            _companyService = companyService;
        }

        /// <summary>
        /// Paginated list of trusted touchpoints based on current filter criteria.
        /// </summary>
        public PaginatedResult<TrustedTouchpointDto> Touchpoints { get; set; } = new();

        /// <summary>
        /// List of companies matching the search term, used for filtering.
        /// </summary>
        public List<CompanyDto> Companies { get; set; } = new();

        /// <summary>
        /// Current filtering and pagination state bound from the query string.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public TrustedTouchpointFilters Filters { get; set; } = new();

        /// <summary>
        /// Optional search term used to filter companies in the dropdown.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? CompanySearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            Touchpoints = await _touchpointService.GetPagedAsync(Filters);

            if (Filters.CompanyId.HasValue)
            {
                var company = await _companyService.GetByIdAsync(Filters.CompanyId.Value);
                if (company != null)
                    Companies = new List<CompanyDto> { company };
            }
            else
            {
                Companies = new List<CompanyDto>();
            }
        }
    }
}
