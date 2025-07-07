using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.Shared;
using LYRA.Server.Models.TrustedTouchpoint;

namespace LYRA.Server.Services.TrustedTouchpoint.Interfaces
{
    /// <summary>
    /// Service interface for managing trusted touchpoints (callers/targets) used in inter-service communication.
    /// </summary>
    public interface ITrustedTouchpointService
    {
        /// <summary>
        /// Searches for active, non-deleted touchpoints by a partial match of their display or system name.
        /// Returns a lightweight list of matching touchpoints (max 10 results), ordered by system name.
        /// </summary>
        /// <param name="term">The search term to match against the touchpoint's display name or system name.</param>
        /// <returns>A list of matching <see cref="TrustedTouchpointDto"/> entries.</returns>
        Task<List<TrustedTouchpointDto>> SearchAsync(string term);

        /// <summary>
        /// Retrieves a lightweight list of trusted touchpoints, typically used for dropdowns or references.
        /// </summary>
        /// <returns>A list of simplified trusted touchpoint DTOs.</returns>
        Task<List<TrustedTouchpointLightDto>> GetLightweightAsync();

        /// <summary>
        /// Retrieves a paginated list of trusted touchpoints based on the provided filters.
        /// </summary>
        /// <param name="filters">Filtering options such as company, mode, or system name.</param>
        /// <returns>A paginated result of trusted touchpoint DTOs.</returns>
        Task<PaginatedResult<TrustedTouchpointDto>> GetPagedAsync(TrustedTouchpointFilters filters);

        /// <summary>
        /// Retrieves a single trusted touchpoint by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the trusted touchpoint.</param>
        /// <returns>The trusted touchpoint DTO if found; otherwise, null.</returns>
        Task<TrustedTouchpointDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new trusted touchpoint to the system.
        /// </summary>
        /// <param name="request">The request object containing creation details.</param>
        /// <returns>The DTO of the created trusted touchpoint including its secret (if applicable).</returns>
        Task<TrustedTouchpointCreatedDto> AddAsync(TrustedTouchpointCreateRequest request);

        /// <summary>
        /// Updates the details of an existing trusted touchpoint.
        /// </summary>
        /// <param name="request">The request object containing updated data.</param>
        Task UpdateAsync(TrustedTouchpointUpdateRequest request);

        /// <summary>
        /// Deletes a trusted touchpoint by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the trusted touchpoint to delete.</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves the total number of trusted touchpoints in the system.
        /// </summary>
        /// <returns>The total count of trusted touchpoints.</returns>
        Task<int> GetTotalTouchpointCountAsync();

        /// <summary>
        /// Checks if a trusted touchpoint with the specified system name already exists.
        /// </summary>
        /// <param name="name">The system name to check.</param>
        /// <param name="excludeId">An optional ID to exclude from the check (useful during updates).</param>
        /// <returns>True if a touchpoint with the name exists; otherwise, false.</returns>
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);

        /// <summary>
        /// Retrieves all trusted touchpoints associated with a specific company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>A list of trusted touchpoints for the given company.</returns>
        Task<List<TrustedTouchpointDto>> GetByCompanyAsync(Guid companyId);

        /// <summary>
        /// Rotates the secret of the trusted touchpoint(s) associated with the specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company whose touchpoint secret should be rotated.</param>
        /// <returns>The result of the secret rotation, including the new secret if applicable.</returns>
        Task<SecretRotationResult?> RotateSecretAsync(Guid companyId);
    }
}
