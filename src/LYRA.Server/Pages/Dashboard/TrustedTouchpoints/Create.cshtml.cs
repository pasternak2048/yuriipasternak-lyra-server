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
    public class CreateModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly ITrustedTouchpointService _touchpointService;

        public CreateModel(ICompanyService companyService, ITrustedTouchpointService touchpointService)
        {
            _companyService = companyService;
            _touchpointService = touchpointService;
        }

        [BindProperty]
        public TrustedTouchpointCreateRequest Input { get; set; } = new();

        public List<CompanyDto> Companies { get; set; } = new();

        public List<SelectListItem> SignatureTypes { get; set; } = new();

        [TempData] 
        public string? SecretPlaintext { get; set; }

        [TempData] 
        public string? DisplayName { get; set; }

        [TempData] 
        public string? SystemName { get; set; }

        [TempData] 
        public string? CompanyName { get; set; }

        [TempData] 
        public string? IsActive { get; set; }

        [TempData] 
        public string? UseCompanySecret { get; set; }

        [TempData] 
        public string? Mode { get; set; }

        [TempData] 
        public string? SignatureType { get; set; }

        [TempData] 
        public string? CreatedAt { get; set; }

        [TempData]
        public Guid Id { get; set; }

        [TempData]
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
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
                var created = await _touchpointService.AddAsync(Input);

                SecretPlaintext = created.SecretPlaintext;
                DisplayName = created.DisplayName;
                SystemName = created.SystemName;
                CompanyName = created.CompanyName;
                IsActive = created.IsActive.ToString();
                UseCompanySecret = created.UseCompanySecret.ToString();
                Mode = created.Mode;
                SignatureType = created.SignatureType;
                CreatedAt = created.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                Id = created.Id;

                Message = $"Trusted Touchpoint '{DisplayName}' created successfully.";
                return RedirectToPage("Secret", new { id = created.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
