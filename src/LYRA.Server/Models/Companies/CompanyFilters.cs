using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.Companies
{
    public class CompanyFilters : PaginationFilter
    {
        public string? Name { get; set; }
    }
}
