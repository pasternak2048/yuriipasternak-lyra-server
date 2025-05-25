using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Companies
{
    public class CompanyCreateRequest
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;


        [MaxLength(200)]
        public string? DisplayName { get; set; }


        [Required]
        public string Secret { get; set; } = null!;
    }
}
