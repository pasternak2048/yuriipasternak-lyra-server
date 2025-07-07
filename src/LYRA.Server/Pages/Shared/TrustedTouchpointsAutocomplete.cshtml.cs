using LYRA.Server.Services.TrustedTouchpoint.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Shared
{
    /// <summary>
    /// Razor Page model for handling autocomplete suggestions for Trusted Touchpoints.
    /// Invoked via AJAX to return matching touchpoints based on the search term.
    /// </summary>
    public class TrustedTouchpointsAutocompleteModel : PageModel
    {
        private readonly ITrustedTouchpointService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrustedTouchpointsAutocompleteModel"/> class.
        /// </summary>
        /// <param name="service">Service used to search for matching touchpoints.</param>
        public TrustedTouchpointsAutocompleteModel(ITrustedTouchpointService service)
        {
            _service = service;
        }

        /// <summary>
        /// Handles GET requests for touchpoint autocomplete.
        /// Accepts a search term and returns a list of matching touchpoints.
        /// </summary>
        /// <param name="term">The search string entered by the user.</param>
        /// <returns>A JSON array of matching touchpoints with id, displayName, and systemName.</returns>
        public async Task<IActionResult> OnGetAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new JsonResult(Array.Empty<object>());

            var results = await _service.SearchAsync(term.Trim());

            var formatted = results.Select(t => new
            {
                id = t.Id,
                displayName = t.DisplayName,
                systemName = t.SystemName
            });

            return new JsonResult(formatted);
        }
    }
}
