using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetLightweightAsync();

        Task<PaginatedResult<CompanyDto>> GetPagedAsync(CompanyFilters filters);

        Task<CompanyDto?> GetByIdAsync(Guid id);

        Task AddAsync(CompanyCreateRequest request);

        Task UpdateAsync(CompanyUpdateRequest request);

        Task DeleteAsync(Guid id);

        Task<int> GetTotalCompanyCountAsync();
    }
}
