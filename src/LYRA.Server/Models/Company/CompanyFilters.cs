using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.Company
{
    public class CompanyFilters : PaginationFilter
    {
        public string? SystemName { get; set; }
    }
}
