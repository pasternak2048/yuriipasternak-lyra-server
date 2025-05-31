using LYRA.Server.Models.Company;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Companies
{
    /// <summary>
    /// Razor Page model for editing a company's basic information.
    /// </summary>
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ICompanyService _companyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditModel"/> class.
        /// </summary>
        public EditModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Form input model containing company data to be edited.
        /// </summary>
        [BindProperty]
        public CompanyUpdateRequest Input { get; set; } = new();

        /// <summary>
        /// Optional status message displayed after update.
        /// </summary>
        [TempData]
        public string? Message { get; set; }

        /// <summary>
        /// Handles GET request to populate the edit form with current company values.
        /// </summary>
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

        /// <summary>
        /// Handles POST request to submit updated company data.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _companyService.UpdateAsync(Input);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            Message = $"Company '{Input.DisplayName}' updated successfully.";
            return RedirectToPage("Details", new { id = Input.Id });
        }
    }
}
