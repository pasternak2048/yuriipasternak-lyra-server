using LYRA.Server.Services.Company.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Shared
{
    /// <summary>
    /// Razor Page that provides a lightweight autocomplete endpoint
    /// for searching companies by display name or system name.
    /// Used by JS-based typeahead components (e.g., in Index/Create views).
    /// </summary>
    [Authorize]
    public class CompaniesAutocompleteModel : PageModel
    {
        private readonly ICompanyService _companyService;

        /// <summary>
        /// Initializes the autocomplete page model with required services.
        /// </summary>
        /// <param name="companyService">Service for querying companies.</param>
        public CompaniesAutocompleteModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Handles GET requests to retrieve company suggestions.
        /// Returns a JSON list of matching companies in format:
        /// { id, displayName, systemName }
        /// </summary>
        /// <param name="term">The search term entered by the user.</param>
        /// <returns>A JSON-formatted list of company entries.</returns>
        public async Task<IActionResult> OnGetAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new JsonResult(new object[0]);

            var results = await _companyService.SearchAsync(term.Trim());

            var formatted = results.Select(c => new
            {
                id = c.Id,
                displayName = c.DisplayName,
                systemName = c.SystemName
            });

            return new JsonResult(formatted);
        }
    }
}
