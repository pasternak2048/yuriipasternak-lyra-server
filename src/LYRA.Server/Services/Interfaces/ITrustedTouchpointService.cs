using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.Shared;
using LYRA.Server.Models.TrustedTouchpoint;

namespace LYRA.Server.Services.Interfaces
{
    public interface ITrustedTouchpointService
    {
        Task<List<TrustedTouchpointLightDto>> GetLightweightAsync();

        Task<PaginatedResult<TrustedTouchpointDto>> GetPagedAsync(TrustedTouchpointFilters filters);

        Task<TrustedTouchpointDto?> GetByIdAsync(Guid id);

        Task<TrustedTouchpointCreatedDto> AddAsync(TrustedTouchpointCreateRequest request);

        Task UpdateAsync(TrustedTouchpointUpdateRequest request);

        Task DeleteAsync(Guid id);

        Task<int> GetTotalTouchpointCountAsync();

        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);

        Task<List<TrustedTouchpointDto>> GetByCompanyAsync(Guid companyId);

        Task<SecretRotationResult?> RotateSecretAsync(Guid companyId);
    }
}
