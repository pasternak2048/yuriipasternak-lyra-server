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
    /// Razor Page model for creating a new access policy between trusted touchpoints.
    /// </summary>
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;
        private readonly ITrustedTouchpointService _touchpointService;
        private readonly ILogger<CreateModel> _logger;

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
        /// Input model bound from form input.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public AccessPolicyCreateRequest Input { get; set; } = new()
        {
            Rules = new List<AccessRuleInput>
            {
                new() { Method = "GET", PathPattern = "/" }
            },
            IsEnabled = true
        };

        /// <summary>
        /// Optional preselected caller ID (provided via query string).
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public Guid? CallerId { get; set; }

        /// <summary>
        /// Optional preselected target ID (provided via query string).
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public Guid? TargetId { get; set; }

        /// <summary>
        /// Preview list containing selected caller (used to render label).
        /// </summary>
        public List<TrustedTouchpointDto> Callers { get; set; } = new();

        /// <summary>
        /// Preview list containing selected target (used to render label).
        /// </summary>
        public List<TrustedTouchpointDto> Targets { get; set; } = new();

        /// <summary>
        /// Handles GET request to initialize form state and pre-fill selected touchpoints.
        /// </summary>
        public async Task OnGetAsync()
        {
            if (CallerId.HasValue)
            {
                var caller = await _touchpointService.GetByIdAsync(CallerId.Value);
                if (caller != null)
                {
                    Callers.Add(caller);
                    Input.CallerId = caller.Id;
                }
            }

            if (TargetId.HasValue)
            {
                var target = await _touchpointService.GetByIdAsync(TargetId.Value);
                if (target != null)
                {
                    Targets.Add(target);
                    Input.TargetId = target.Id;
                }
            }
        }

        /// <summary>
        /// Handles POST request to validate input and create a new access policy.
        /// </summary>
        /// <returns>Redirects to index on success, or redisplays form on failure.</returns>
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

        private async Task LoadSelectedTouchpointsAsync()
        {
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
