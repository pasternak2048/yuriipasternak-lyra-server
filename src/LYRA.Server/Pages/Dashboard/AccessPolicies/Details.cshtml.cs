using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.AccessPolicies
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IAccessPolicyService _policyService;

        public DetailsModel(IAccessPolicyService policyService)
        {
            _policyService = policyService;
        }

        public AccessPolicyDto? Policy { get; set; }

        [TempData]
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Policy = await _policyService.GetByIdAsync(id);
            if (Policy == null)
                return NotFound();

            return Page();
        }

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
