using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Company
{
    public class CompanyUpdateRequest : CompanyCreateRequest
    {
        [Required]
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
    }
}
