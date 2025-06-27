using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.Company.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    /// <summary>
    /// Razor Page model for displaying a paginated list of companies with optional filtering.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICompanyService _companyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        public IndexModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Filtering and pagination parameters bound from query string.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public CompanyFilters Filters { get; set; } = new();

        /// <summary>
        /// Result set containing companies matching the filters.
        /// </summary>
        public PaginatedResult<CompanyDto> Companies { get; set; } = new();

        /// <summary>
        /// Handles GET requests to populate the company list.
        /// </summary>
        public async Task OnGetAsync()
        {
            if (Filters.Page < 1)
                Filters.Page = 1;

            if (Filters.PageSize < 1)
                Filters.PageSize = 10;

            Companies = await _companyService.GetPagedAsync(Filters);
        }
    }
}
