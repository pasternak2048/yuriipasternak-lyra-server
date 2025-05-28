using LYRA.Server.Enums;
using LYRA.Server.Models.Company;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _touchpointService;

        public EditModel(ICompanyService companyService, ITrustedTouchpointService touchpointService)
        {
            _companyService = companyService;
            _touchpointService = touchpointService;
        }

        [BindProperty]
        public TrustedTouchpointUpdateRequest Input { get; set; } = new();

        public List<CompanyDto> Companies { get; set; } = new();

        public List<SelectListItem> SignatureTypes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var touchpoint = await _touchpointService.GetByIdAsync(id);
            if (touchpoint == null) return NotFound();

            Input = new TrustedTouchpointUpdateRequest
            {
                Id = touchpoint.Id,
                CompanyId = touchpoint.CompanyId,
                DisplayName = touchpoint.DisplayName,
                IsActive = touchpoint.IsActive,
                UseCompanySecret = touchpoint.UseCompanySecret,
                Mode = touchpoint.Mode.ToString(),
                SignatureType = touchpoint.SignatureType.ToString(),
                Description = touchpoint.Description,
                AllowedSourceIp = touchpoint.AllowedSourceIp
            };

            Companies = await _companyService.GetLightweightAsync();
            SignatureTypes = EnumHelper.GetSelectList<SignatureType>();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            SignatureTypes = EnumHelper.GetSelectList<SignatureType>();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _touchpointService.UpdateAsync(Input);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Input.DisplayName", ex.Message);
                return Page();
            }

            return RedirectToPage("Details", new { id = Input.Id });
        }
    }
}
