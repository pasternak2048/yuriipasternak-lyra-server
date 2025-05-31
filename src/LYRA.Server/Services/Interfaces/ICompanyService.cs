using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.Shared;

namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing registered companies in the security system.
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// Retrieves a lightweight list of companies, typically used for dropdowns or selection lists.
        /// </summary>
        /// <returns>A list of companies with minimal information (e.g., name and ID).</returns>
        Task<List<CompanyDto>> GetLightweightAsync();

        /// <summary>
        /// Retrieves a paginated list of companies based on the specified filters.
        /// </summary>
        /// <param name="filters">Filtering options such as name or creation date.</param>
        /// <returns>A paginated result of company DTOs.</returns>
        Task<PaginatedResult<CompanyDto>> GetPagedAsync(CompanyFilters filters);

        /// <summary>
        /// Retrieves a single company by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <returns>The company DTO if found; otherwise, null.</returns>
        Task<CompanyDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new company to the system.
        /// </summary>
        /// <param name="request">The request object containing company creation details.</param>
        /// <returns>A DTO containing the created company's details including the generated secret.</returns>
        Task<CompanyCreatedDto> AddAsync(CompanyCreateRequest request);

        /// <summary>
        /// Updates an existing company.
        /// </summary>
        /// <param name="request">The request object containing updated company data.</param>
        Task UpdateAsync(CompanyUpdateRequest request);

        /// <summary>
        /// Deletes a company by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the company to delete.</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves the total number of registered companies in the system.
        /// </summary>
        /// <returns>The total count of companies.</returns>
        Task<int> GetTotalCompanyCountAsync();

        /// <summary>
        /// Rotates the secret key associated with the specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company for which the secret should be rotated.</param>
        /// <returns>The result of the secret rotation, including the new secret if applicable.</returns>
        Task<SecretRotationResult?> RotateSecretAsync(Guid companyId);
    }
}
