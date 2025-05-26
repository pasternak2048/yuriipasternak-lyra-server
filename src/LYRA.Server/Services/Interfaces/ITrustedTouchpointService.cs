using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.TrustedTouchpoint;

namespace LYRA.Server.Services.Interfaces
{
    public interface ITrustedTouchpointService
    {
        Task<PaginatedResult<TrustedTouchpointDto>> GetPagedAsync(TrustedTouchpointFilters filters);

        Task<TrustedTouchpointDto?> GetByIdAsync(Guid id);

        Task AddAsync(TrustedTouchpointCreateRequest request);

        Task UpdateAsync(TrustedTouchpointUpdateRequest request);

        Task DeleteAsync(Guid id);

        Task<int> GetTotalTouchpointCountAsync();

        Task<List<TrustedTouchpointDto>> GetByCompanyAsync(Guid companyId);
    }
}
