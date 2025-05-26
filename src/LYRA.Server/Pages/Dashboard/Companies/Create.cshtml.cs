using LYRA.Server.Models.Company;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ICompanyService _companyService;

        public CreateModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [BindProperty]
        public CompanyCreateRequest Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var normalizedName = SlugHelper.Slugify(Input.DisplayName);
            var exists = await _companyService.ExistsByNameAsync(normalizedName);

            if (exists)
            {
                ModelState.AddModelError("Input.DisplayName", "A company with this display name already exists.");
                return Page();
            }

            await _companyService.AddAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
