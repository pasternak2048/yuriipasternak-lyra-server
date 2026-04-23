using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Pages.Dashboard.AccessPolicies
{
    /// <summary>
    /// Razor Page model for editing an existing access policy between trusted touchpoints.
    /// </summary>
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ILogger<EditModel> _logger;

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
        /// Input model for editing.
        /// </summary>
        [BindProperty]
        public AccessPolicyUpdateRequest Input { get; set; } = new();

        /// <summary>
        /// Caller preview list (for rendering autocomplete label).
        /// </summary>
        public List<TrustedTouchpointDto> Callers { get; set; } = new();

        /// <summary>
        /// Target preview list (for rendering autocomplete label).
        /// </summary>
        public List<TrustedTouchpointDto> Targets { get; set; } = new();

        /// <summary>
        /// Loads the edit form with access policy and related touchpoints.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var policy = await _policyService.GetByIdAsync(id);
            if (policy == null)
                return NotFound();

            Input = new AccessPolicyUpdateRequest
            {
                Id = policy.Id,
                CallerId = policy.CallerId,
                TargetId = policy.TargetId,
                Rules = policy.Rules
                    .Select(r => new AccessRuleInput
                    {
                        Method = r.Method == "*" ? "ANY" : r.Method,
                        PathPattern = r.PathPattern
                    })
                    .ToList(),
                IsEnabled = policy.IsEnabled
            };

            await LoadSelectedTouchpointsAsync();

            return Page();
        }

        /// <summary>
        /// Handles saving the updated policy.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSelectedTouchpointsAsync();

            Input.Rules = Input.Rules
                .Where(r => r is not null)
                .Where(r => !string.IsNullOrWhiteSpace(r.Method) || !string.IsNullOrWhiteSpace(r.PathPattern))
                .ToList();

            if (Input.Rules.Count == 0)
                ModelState.AddModelError(string.Empty, "At least one route is required.");

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

        private async Task LoadSelectedTouchpointsAsync()
        {
            Callers.Clear();
            Targets.Clear();

            if (Input.CallerId.HasValue)
            {
                var caller = await _touchpointService.GetByIdAsync(Input.CallerId.Value);
                if (caller != null)
                    Callers.Add(caller);
            }

            if (Input.TargetId.HasValue)
            {
                var target = await _touchpointService.GetByIdAsync(Input.TargetId.Value);
                if (target != null)
                    Targets.Add(target);
            }
        }
    }
}
