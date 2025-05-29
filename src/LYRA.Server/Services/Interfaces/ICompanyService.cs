using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.Shared;

namespace LYRA.Server.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetLightweightAsync();

        Task<PaginatedResult<CompanyDto>> GetPagedAsync(CompanyFilters filters);

        Task<CompanyDto?> GetByIdAsync(Guid id);

        Task<CompanyCreatedDto> AddAsync(CompanyCreateRequest request);

        Task UpdateAsync(CompanyUpdateRequest request);

        Task DeleteAsync(Guid id);

        Task<int> GetTotalCompanyCountAsync();

        Task<SecretRotationResult?> RotateSecretAsync(Guid companyId);

        Task<bool> ExistsByDisplayNameAsync(string displayName, Guid? excludeId = null);
    }
}
