using LYRA.Server.Models.Companies;
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _companyService.AddAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
