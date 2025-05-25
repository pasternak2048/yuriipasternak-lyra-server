using LYRA.Server.Models.Agents;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Services.Interfaces
{
    public interface ITrustedAgentService
    {
        Task<PaginatedResult<TrustedAgentDto>> GetPagedAsync(TrustedAgentFilters filters);

        Task<TrustedAgentDto?> GetByIdAsync(Guid id);

        Task AddAsync(TrustedAgentCreateRequest request);

        Task UpdateAsync(TrustedAgentUpdateRequest request);

        Task DeleteAsync(Guid id);

        Task<int> GetTotalAgentCountAsync();

        Task<List<TrustedAgentDto>> GetByCompanyAsync(Guid companyId);
    }
}
