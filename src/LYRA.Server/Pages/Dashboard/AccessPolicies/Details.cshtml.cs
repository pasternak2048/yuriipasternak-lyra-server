using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.AccessPolicies
{
    /// <summary>
    /// Razor Page model for viewing and deleting a specific access policy.
    /// </summary>
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailsModel"/> class.
        /// </summary>
        /// <param name="policyService">The service used to manage access policies.</param>
        public DetailsModel(IAccessPolicyService policyService)
        {
            _policyService = policyService;
        }

        /// <summary>
        /// The access policy details to display.
        /// </summary>
        public AccessPolicyDto? Policy { get; set; }

        /// <summary>
        /// Optional success message shown in the UI.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Handles GET requests to load policy details by ID.
        /// </summary>
        /// <param name="id">The ID of the policy to retrieve.</param>
        /// <returns>The details page or NotFound if not found.</returns>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Policy = await _policyService.GetByIdAsync(id);
            if (Policy == null)
                return NotFound();

            return Page();
        }

        /// <summary>
        /// Handles POST requests to delete a policy.
        /// </summary>
        /// <param name="id">The ID of the policy to delete.</param>
        /// <returns>Redirect to index on success or redisplay page on error.</returns>
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                await _policyService.DeleteAsync(id);
                Message = "Access policy deleted successfully.";
                return RedirectToPage("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Policy = await _policyService.GetByIdAsync(id);
                return Page();
            }
        }
    }
}
