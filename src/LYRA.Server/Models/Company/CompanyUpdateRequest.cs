using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.Company
{
    /// <summary>
    /// Request model for updating a company.
    /// Inherits from <see cref="CompanyCreateRequest"/> and adds update-specific fields.
    /// </summary>
    public class CompanyUpdateRequest : CompanyCreateRequest
    {
        /// <summary>
        /// Unique identifier of the company to update.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Indicates whether the company is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
