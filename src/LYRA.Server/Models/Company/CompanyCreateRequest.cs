using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Company
{
    /// <summary>
    /// Request model for creating a new company.
    /// </summary>
    public class CompanyCreateRequest
    {
        /// <summary>
        /// Human-readable display name of the company.
        /// Required. Maximum length: 200 characters.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = null!;
    }
}
