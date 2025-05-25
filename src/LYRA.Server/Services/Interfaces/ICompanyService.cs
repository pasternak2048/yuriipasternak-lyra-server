using LYRA.Server.Models.Companies;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<PaginatedResult<CompanyDto>> GetPagedAsync(CompanyFilters filters);

        Task<CompanyDto?> GetByIdAsync(Guid id);

        Task AddAsync(CompanyCreateRequest request);

        Task UpdateAsync(CompanyUpdateRequest request);

        Task DeleteAsync(Guid id);
    }
}
