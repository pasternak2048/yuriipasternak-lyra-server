using LYRA.Server.Models.Company;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ICompanyService _companyService;

        public DeleteModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public CompanyDto? Company { get; set; }

        [BindProperty]
        public Guid CompanyId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Company = await _companyService.GetByIdAsync(id);
            if (Company == null)
                return NotFound();

            CompanyId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (CompanyId == Guid.Empty)
                return BadRequest();

            await _companyService.DeleteAsync(CompanyId);
            return RedirectToPage("Index");
        }
    }
}
