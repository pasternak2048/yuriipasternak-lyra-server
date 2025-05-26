using LYRA.Server.Models.Company;
using LYRA.Server.Services.Interfaces;
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
                Name = company.Name,
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

            await _companyService.UpdateAsync(Input);
            return RedirectToPage("Index");
        }
    }
}
