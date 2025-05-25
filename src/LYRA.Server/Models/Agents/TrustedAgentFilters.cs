using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.Agents
{
    public class TrustedAgentFilters : PaginationFilter
    {
        public string? Name { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
