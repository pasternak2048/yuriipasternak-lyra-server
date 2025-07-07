using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
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
        private readonly ITrustedTouchpointService _touchpointService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="policyService">Service for managing access policies.</param>
        /// <param name="touchpointService">Service for retrieving trusted touchpoints.</param>
        public IndexModel(IAccessPolicyService policyService, ITrustedTouchpointService touchpointService)
        {
            _policyService = policyService;
            _touchpointService = touchpointService;
        }

        /// <summary>
        /// List of caller touchpoints used to display selected values in filters.
        /// </summary>
        public List<TrustedTouchpointDto> Callers { get; set; } = new();

        /// <summary>
        /// List of target touchpoints used to display selected values in filters.
        /// </summary>
        public List<TrustedTouchpointDto> Targets { get; set; } = new();

        /// <summary>
        /// List of access policies matching the current filter.
        /// </summary>
        public PaginatedResult<AccessPolicyDto> Policies { get; set; } = new();

        /// <summary>
        /// Current filter and pagination state, bound from query parameters.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public AccessPolicyFilters Filters { get; set; } = new();

        /// <summary>
        /// Handles GET request to load access policies based on filters.
        /// Populates Callers and Targets to resolve labels for selected IDs.
        /// </summary>
        public async Task OnGetAsync()
        {
            Policies = await _policyService.GetPagedAsync(Filters);

            if (Filters.CallerId.HasValue)
            {
                var tp = await _touchpointService.GetByIdAsync(Filters.CallerId.Value);
                if (tp != null)
                    Callers.Add(tp);
            }

            if (Filters.TargetId.HasValue)
            {
                var tp = await _touchpointService.GetByIdAsync(Filters.TargetId.Value);
                if (tp != null)
                    Targets.Add(tp);
            }
        }
    }
}
