using LYRA.Server.Models.Company;
using LYRA.Server.Services.Company.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    /// <summary>
    /// Razor Page model for creating a new company.
    /// </summary>
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ICompanyService _companyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateModel"/> class.
        /// </summary>
        public CreateModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Form input data for creating the company.
        /// </summary>
        [BindProperty]
        public CompanyCreateRequest Input { get; set; } = new();

        /// <summary>
        /// Plaintext version of the generated secret (shown once).
        /// </summary>
        [TempData]
        public string? SecretPlaintext { get; set; }

        /// <summary>
        /// Display name of the created company.
        /// </summary>
        [TempData]
        public string? DisplayName { get; set; }

        /// <summary>
        /// System name (slug) of the created company.
        /// </summary>
        [TempData]
        public string? SystemName { get; set; }

        /// <summary>
        /// Activation status as a string.
        /// </summary>
        [TempData]
        public string? IsActive { get; set; }

        /// <summary>
        /// Timestamp when the company was created.
        /// </summary>
        [TempData]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// ID of the newly created company.
        /// </summary>
        [TempData]
        public Guid Id { get; set; }

        /// <summary>
        /// Notification message displayed to the user.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Handles POST request for creating a company.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var created = await _companyService.AddAsync(Input);

                SecretPlaintext = created.SecretPlaintext;
                DisplayName = created.DisplayName;
                SystemName = created.SystemName;
                IsActive = created.IsActive.ToString();
                CreatedAt = created.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                Id = created.Id;

                Message = $"Company '{DisplayName}' created successfully.";
                return RedirectToPage("Secret", new { id = created.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
