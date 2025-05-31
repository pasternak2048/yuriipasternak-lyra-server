using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing access policies between trusted touchpoints.
    /// </summary>
    public interface IAccessPolicyService
    {
        /// <summary>
        /// Retrieves a paginated list of access policies based on the specified filters.
        /// </summary>
        /// <param name="filters">Filtering options such as context, caller, target, or operation.</param>
        /// <returns>A paginated result of access policy DTOs.</returns>
        Task<PaginatedResult<AccessPolicyDto>> GetPagedAsync(AccessPolicyFilters filters);

        /// <summary>
        /// Retrieves a single access policy by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the access policy.</param>
        /// <returns>The access policy DTO if found; otherwise, null.</returns>
        Task<AccessPolicyDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new access policy to the system.
        /// </summary>
        /// <param name="request">The request object containing policy details.</param>
        /// <returns>The created access policy DTO.</returns>
        Task<AccessPolicyDto> AddAsync(AccessPolicyCreateRequest request);

        /// <summary>
        /// Updates an existing access policy.
        /// </summary>
        /// <param name="request">The request object containing updated policy data.</param>
        Task UpdateAsync(AccessPolicyUpdateRequest request);

        /// <summary>
        /// Deletes an access policy by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the access policy to delete.</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves the total number of access policies in the system.
        /// </summary>
        /// <returns>The total count of policies.</returns>
        Task<int> GetTotalPolicyCountAsync();
    }
}
