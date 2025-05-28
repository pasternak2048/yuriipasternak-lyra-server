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

        [TempData] public Guid Id { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var normalizedName = NameHelper.EnsureSlug(Input.DisplayName, "company");

            var exists = await _companyService.ExistsByNameAsync(normalizedName);
            if (exists)
            {
                ModelState.AddModelError("Input.DisplayName", "A company with this system name already exists.");
                return Page();
            }

            var created = await _companyService.AddAsync(Input);

            SecretPlaintext = created.SecretPlaintext;
            DisplayName = created.DisplayName;
            Name = created.Name;
            IsActive = created.IsActive.ToString();
            CreatedAt = created.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            Id = created.Id;

            return RedirectToPage("Secret", new { id = created.Id });
        }
    }
}
