using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Services.AccessPolicy.Interfaces
{
    /// <summary>
    /// Service interface for managing access policies between trusted touchpoints.
    /// </summary>
    public interface IAccessPolicyService
    {
        /// <summary>
        /// Retrieves a paginated list of access policies based on the specified filters.
        /// </summary>
        Task<PaginatedResult<AccessPolicyDto>> GetPagedAsync(AccessPolicyFilters filters);

        /// <summary>
        /// Retrieves a single access policy by its unique identifier.
        /// </summary>
        Task<AccessPolicyDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new access policy to the system.
        /// </summary>
        Task<AccessPolicyDto> AddAsync(AccessPolicyCreateRequest request);

        /// <summary>
        /// Updates an existing access policy.
        /// </summary>
        Task UpdateAsync(AccessPolicyUpdateRequest request);

        /// <summary>
        /// Deletes an access policy by its unique identifier.
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves the total number of access policies in the system.
        /// </summary>
        Task<int> GetTotalPolicyCountAsync();

        /// <summary>
        /// Determines if the caller is allowed to access the target for a given method and path.
        /// </summary>
        Task<bool> IsAuthorizedAsync(string caller, string target, string method, string path);
    }
}
