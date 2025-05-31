using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    /// <summary>
    /// Razor Page model for displaying the generated secret after company creation or rotation.
    /// </summary>
    [Authorize]
    public class SecretModel : PageModel
    {
        /// <summary>
        /// Plaintext secret value to be shown once to the user.
        /// </summary>
        [TempData]
        public string? SecretPlaintext { get; set; }

        /// <summary>
        /// Display name of the related company.
        /// </summary>
        [TempData]
        public string? DisplayName { get; set; }

        /// <summary>
        /// System name of the related company.
        /// </summary>
        [TempData]
        public string? SystemName { get; set; }

        /// <summary>
        /// Company active status in string format (used with TempData).
        /// </summary>
        [TempData]
        public string? IsActive { get; set; }

        /// <summary>
        /// UTC creation timestamp of the company.
        /// </summary>
        [TempData]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// Unique identifier of the company.
        /// </summary>
        [TempData]
        public Guid Id { get; set; }

        /// <summary>
        /// Optional user-facing message stored in TempData.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Boolean representation of the company's active status.
        /// </summary>
        public bool IsActiveBool => bool.TryParse(IsActive, out var active) && active;

        /// <summary>
        /// Displays the page if required data is present in TempData, otherwise redirects to Index.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (string.IsNullOrWhiteSpace(SecretPlaintext) || string.IsNullOrWhiteSpace(DisplayName))
                return RedirectToPage("Index");

            return Page();
        }
    }
}
