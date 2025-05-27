using LYRA.Server.Models.Company;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ICompanyService _companyService;

        public EditModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [BindProperty]
        public CompanyUpdateRequest Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null)
                return NotFound();

            Input = new CompanyUpdateRequest
            {
                Id = company.Id,
                DisplayName = company.DisplayName,
                IsActive = company.IsActive
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var normalizedName = SlugHelper.Slugify(Input.DisplayName);

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                ModelState.AddModelError("Input.DisplayName", "Display name must contain at least one alphanumeric character.");
                return Page();
            }

            var exists = await _companyService.ExistsByNameAsync(normalizedName, Input.Id);

            if (exists)
            {
                ModelState.AddModelError("Input.DisplayName", "Another company with this display name already exists.");
                return Page();
            }

            await _companyService.UpdateAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
