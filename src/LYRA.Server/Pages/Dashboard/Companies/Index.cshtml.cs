using LYRA.Server.Models.Companies;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICompanyService _companyService;

        public IndexModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [BindProperty(SupportsGet = true)]
        public CompanyFilters Filters { get; set; } = new();

        public PaginatedResult<CompanyDto> Companies { get; set; } = new();

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
