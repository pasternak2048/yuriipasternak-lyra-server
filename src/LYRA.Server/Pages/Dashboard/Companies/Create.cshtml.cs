using LYRA.Server.Models.Company;
using LYRA.Server.Services.Interfaces;
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var created = await _companyService.AddAsync(Input);

            SecretPlaintext = created.SecretPlaintext;
            DisplayName = created.DisplayName;
            Name = created.Name;
            IsActive = created.IsActive.ToString();
            CreatedAt = created.CreatedAt.ToString("yyyy-MM-dd HH:mm");

            return RedirectToPage("Secret", new { id = created.Id });
        }
    }
}
