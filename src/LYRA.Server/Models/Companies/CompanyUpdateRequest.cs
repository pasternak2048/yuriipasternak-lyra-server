using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Companies
{
    public class CompanyUpdateRequest : CompanyCreateRequest
    {
        [Required]
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
    }
}
