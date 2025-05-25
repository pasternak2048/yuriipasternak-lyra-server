using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.Companies
{
    public class CompanyFilters : PaginationFilter
    {
        public string? SearchTerm { get; set; }
    }
}
