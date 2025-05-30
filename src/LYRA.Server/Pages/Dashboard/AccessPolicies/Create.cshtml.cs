using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Pages.Dashboard.AccessPolicies
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IAccessPolicyService policyService, ITrustedTouchpointService touchpointService, ILogger<CreateModel> logger)
        {
            _policyService = policyService;
            _touchpointService = touchpointService;
            _logger = logger;
        }

        [BindProperty]
        public AccessPolicyCreateRequest Input { get; set; } = new();

        public List<TrustedTouchpointLightDto> Touchpoints { get; set; } = new();

        public SelectList TouchpointSelectList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Touchpoints = await _touchpointService.GetLightweightAsync();
            TouchpointSelectList = new SelectList(Touchpoints, nameof(TrustedTouchpointDto.SystemName), nameof(TrustedTouchpointDto.SystemName));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Touchpoints = await _touchpointService.GetLightweightAsync();
            TouchpointSelectList = new SelectList(Touchpoints, nameof(TrustedTouchpointDto.SystemName), nameof(TrustedTouchpointDto.SystemName));

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
