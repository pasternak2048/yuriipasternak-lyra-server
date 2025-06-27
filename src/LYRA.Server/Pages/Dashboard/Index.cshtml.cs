using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.Company.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard
{
    /// <summary>
    /// Razor Page model for the dashboard home page.
    /// Displays statistics about companies, touchpoints, and access policies.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _trustedTouchpointService;
        private readonly IAccessPolicyService _accessPolicyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="companyService">Service for managing companies.</param>
        /// <param name="trustedTouchpointService">Service for managing trusted touchpoints.</param>
        /// <param name="accessPolicyService">Service for managing access policies.</param>
        public IndexModel(
            ICompanyService companyService,
            ITrustedTouchpointService trustedTouchpointService,
            IAccessPolicyService accessPolicyService)
        {
            _companyService = companyService;
            _trustedTouchpointService = trustedTouchpointService;
            _accessPolicyService = accessPolicyService;
        }

        /// <summary>
        /// Total number of companies in the system.
        /// </summary>
        public int CompanyCount { get; set; }

        /// <summary>
        /// Total number of trusted touchpoints.
        /// </summary>
        public int TouchpointCount { get; set; }

        /// <summary>
        /// Total number of access policies defined.
        /// </summary>
        public int PolicyCount { get; set; }

        /// <summary>
        /// Handles GET request for the dashboard page.
        /// Loads and displays summary statistics.
        /// </summary>
        public async Task OnGetAsync()
        {
            CompanyCount = await _companyService.GetTotalCompanyCountAsync();
            TouchpointCount = await _trustedTouchpointService.GetTotalTouchpointCountAsync();
            PolicyCount = await _accessPolicyService.GetTotalPolicyCountAsync();
        }
    }
}
