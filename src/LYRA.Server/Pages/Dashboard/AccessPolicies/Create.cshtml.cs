using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Pages.Dashboard.AccessPolicies
{
    /// <summary>
    /// Razor Page model for creating a new access policy.
    /// </summary>
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ILogger<CreateModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateModel"/> class.
        /// </summary>
        public CreateModel(
            IAccessPolicyService policyService,
            ITrustedTouchpointService touchpointService,
            ILogger<CreateModel> logger)
        {
            _policyService = policyService;
            _touchpointService = touchpointService;
            _logger = logger;
        }

        /// <summary>
        /// The form input model bound to the page.
        /// </summary>
        [BindProperty]
        public AccessPolicyCreateRequest Input { get; set; } = new();

        /// <summary>
        /// Lightweight list of trusted touchpoints used for dropdowns.
        /// </summary>
        public List<TrustedTouchpointLightDto> Touchpoints { get; set; } = new();

        /// <summary>
        /// Select list used in the UI for choosing touchpoints.
        /// </summary>
        public SelectList TouchpointSelectList { get; set; } = default!;

        /// <summary>
        /// Handles the GET request, initializing the dropdown list.
        /// </summary>
        public async Task OnGetAsync()
        {
            Touchpoints = await _touchpointService.GetLightweightAsync();
            TouchpointSelectList = new SelectList(
                Touchpoints,
                nameof(TrustedTouchpointDto.SystemName),
                nameof(TrustedTouchpointDto.SystemName));
        }

        /// <summary>
        /// Handles the POST request to create the new policy.
        /// Performs validation and invokes the service layer.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            Touchpoints = await _touchpointService.GetLightweightAsync();
            TouchpointSelectList = new SelectList(
                Touchpoints,
                nameof(TrustedTouchpointDto.SystemName),
                nameof(TrustedTouchpointDto.SystemName));

            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _policyService.AddAsync(Input);
                TempData["Message"] = "Access policy created successfully.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create access policy.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
