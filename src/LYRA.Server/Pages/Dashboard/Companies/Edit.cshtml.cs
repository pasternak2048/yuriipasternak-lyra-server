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
                Secret = company.Secret,
                IsActive = company.IsActive
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var normalizedName = SlugHelper.Slugify(Input.DisplayName);
            var existing = await _companyService.GetLightweightAsync();

            if (existing.Any(c => c.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Input.DisplayName", "A company with this display name already exists.");
                return Page();
            }

            await _companyService.UpdateAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
