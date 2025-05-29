using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.TrustedTouchpoints
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ITrustedTouchpointService _touchpointService;

        public DetailsModel(ITrustedTouchpointService touchpointService)
        {
            _touchpointService = touchpointService;
        }

        public TrustedTouchpointDto? Touchpoint { get; set; }

        [TempData] 
        public string? SecretPlaintext { get; set; }

        [TempData] 
        public string? DisplayName { get; set; }

        [TempData] 
        public string? Name { get; set; }

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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Touchpoint = await _touchpointService.GetByIdAsync(id);
            if (Touchpoint == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostRotateAsync(Guid id)
        {
            var result = await _touchpointService.RotateSecretAsync(id);
            if (result == null) return NotFound();

            var touchpoint = await _touchpointService.GetByIdAsync(id);
            if (touchpoint == null) return NotFound();

            SecretPlaintext = result.SecretPlaintext;
            DisplayName = touchpoint.DisplayName;
            Name = touchpoint.Name;
            CompanyName = touchpoint.CompanyName;
            IsActive = touchpoint.IsActive.ToString();
            UseCompanySecret = touchpoint.UseCompanySecret.ToString();
            Mode = touchpoint.Mode.ToString();
            SignatureType = touchpoint.SignatureType.ToString();
            CreatedAt = touchpoint.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            Id = touchpoint.Id;

            return RedirectToPage("Secret", new { id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _touchpointService.DeleteAsync(id);
            Message = $"Trusted Touchpoint deleted successfully.";
            return RedirectToPage("Index");
        }
    }
}
