using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Pages.Dashboard.AccessPolicies
{
    /// <summary>
    /// Razor Page model for editing an existing access policy.
    /// </summary>
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ILogger<EditModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditModel"/> class.
        /// </summary>
        public EditModel(
            IAccessPolicyService policyService,
            ITrustedTouchpointService touchpointService,
            ILogger<EditModel> logger)
        {
            _policyService = policyService;
            _touchpointService = touchpointService;
            _logger = logger;
        }

        /// <summary>
        /// Form input model for updating the access policy.
        /// </summary>
        [BindProperty]
        public AccessPolicyUpdateRequest Input { get; set; } = new();

        /// <summary>
        /// Lightweight list of touchpoints for dropdown selection.
        /// </summary>
        public List<TrustedTouchpointLightDto> Touchpoints { get; set; } = new();

        /// <summary>
        /// SelectList used to populate dropdowns in the UI.
        /// </summary>
        public SelectList TouchpointSelectList { get; set; } = default!;

        /// <summary>
        /// Loads the access policy for editing.
        /// </summary>
        /// <param name="id">The ID of the access policy to edit.</param>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var policy = await _policyService.GetByIdAsync(id);
            if (policy == null)
                return NotFound();

            Input = new AccessPolicyUpdateRequest
            {
                Id = policy.Id,
                CallerSystemName = policy.CallerSystemName,
                TargetSystemName = policy.TargetSystemName,
                Operation = policy.Operation,
                Context = policy.Context,
                IsEnabled = policy.IsEnabled
            };

            Touchpoints = await _touchpointService.GetLightweightAsync();
            TouchpointSelectList = new SelectList(Touchpoints, nameof(TrustedTouchpointDto.SystemName), nameof(TrustedTouchpointDto.SystemName));

            return Page();
        }

        /// <summary>
        /// Handles the POST request to update the access policy.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            Touchpoints = await _touchpointService.GetLightweightAsync();
            TouchpointSelectList = new SelectList(Touchpoints, nameof(TrustedTouchpointDto.SystemName), nameof(TrustedTouchpointDto.SystemName));

            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _policyService.UpdateAsync(Input);
                TempData["Message"] = "Access policy updated successfully.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update access policy.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
