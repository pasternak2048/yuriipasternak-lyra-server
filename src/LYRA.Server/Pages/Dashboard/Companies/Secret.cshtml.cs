using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    [Authorize]
    public class SecretModel : PageModel
    {
        [TempData] 
        public string? SecretPlaintext { get; set; }

        [TempData] 
        public string? DisplayName { get; set; }

        [TempData] 
        public string? Name { get; set; }

        [TempData] 
        public string? IsActive { get; set; }

        [TempData] 
        public string? CreatedAt { get; set; }

        [TempData] 
        public Guid Id { get; set; }

        [TempData]
        public string? Message { get; set; }

        public bool IsActiveBool => bool.TryParse(IsActive, out var active) && active;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (string.IsNullOrWhiteSpace(SecretPlaintext) || string.IsNullOrWhiteSpace(DisplayName))
                return RedirectToPage("Index");

            return Page();
        }
    }
}
