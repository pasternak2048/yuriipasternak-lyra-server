using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Services.Interfaces
{
    public interface IAccessPolicyService
    {
        Task<PaginatedResult<AccessPolicyDto>> GetPagedAsync(AccessPolicyFilters filters);

        Task<AccessPolicyDto?> GetByIdAsync(Guid id);

        Task<AccessPolicyDto> AddAsync(AccessPolicyCreateRequest request);

        Task DeleteAsync(Guid id);
    }
}
