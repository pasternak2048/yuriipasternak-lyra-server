using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.AccessPolicies
{
    /// <summary>
    /// Razor Page model for listing and filtering access policies.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        public IndexModel(IAccessPolicyService policyService)
        {
            _policyService = policyService;
        }

        /// <summary>
        /// List of access policies matching the current filter.
        /// </summary>
        public PaginatedResult<AccessPolicyDto> Policies { get; set; } = new();

        /// <summary>
        /// Current filter and pagination state bound from query parameters.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public AccessPolicyFilters Filters { get; set; } = new();

        /// <summary>
        /// Handles GET request to load filtered access policies.
        /// </summary>
        public async Task OnGetAsync()
        {
            Policies = await _policyService.GetPagedAsync(Filters);
        }
    }
}
