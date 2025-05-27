using LYRA.Server.Models.Company;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        [TempData] public string? SecretPlaintext { get; set; }

        [TempData] public string? DisplayName { get; set; }

        [TempData] public string? Name { get; set; }

        [TempData] public string? CompanyName { get; set; }

        [TempData] public string? IsActive { get; set; }

        [TempData] public string? UseCompanySecret { get; set; }

        [TempData] public string? Mode { get; set; }

        [TempData] public string? SignatureType { get; set; }

        [TempData] public string? CreatedAt { get; set; }

        [TempData] public Guid Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Companies = await _companyService.GetLightweightAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Companies = await _companyService.GetLightweightAsync();

            if (!ModelState.IsValid)
                return Page();

            var normalizedName = SlugHelper.Slugify(Input.DisplayName);

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                ModelState.AddModelError("Input.DisplayName", "Display name must contain at least one alphanumeric character.");
                return Page();
            }

            var exists = await _touchpointService.ExistsByCompanyAndNameAsync(Input.CompanyId, normalizedName);

            if (exists)
            {
                ModelState.AddModelError("Input.DisplayName", "A touchpoint with this name already exists in the selected company.");
                return Page();
            }

            var created = await _touchpointService.AddAsync(Input);

            SecretPlaintext = created.SecretPlaintext;
            DisplayName = created.DisplayName;
            Name = created.Name;
            CompanyName = created.CompanyName;
            IsActive = created.IsActive.ToString();
            UseCompanySecret = created.UseCompanySecret.ToString();
            Mode = created.Mode.ToString();
            SignatureType = created.SignatureType.ToString();
            CreatedAt = created.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            Id = created.Id;

            return RedirectToPage("Secret", new { id = created.Id });
        }
    }
}
