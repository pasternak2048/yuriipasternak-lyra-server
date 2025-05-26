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

        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);

        Task<SecretRotationResult?> RotateSecretAsync(Guid companyId);
    }
}
