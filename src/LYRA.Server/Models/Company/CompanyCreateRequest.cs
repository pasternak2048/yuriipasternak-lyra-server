using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Company
{
    public class CompanyCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = null!;
    }
}
