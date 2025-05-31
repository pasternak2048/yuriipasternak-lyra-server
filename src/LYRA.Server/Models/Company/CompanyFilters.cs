using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.Company
{
    /// <summary>
    /// Filters for querying companies with pagination support.
    /// </summary>
    public class CompanyFilters : PaginationFilter
    {
        /// <summary>
        /// Optional system name filter (partial match).
        /// </summary>
        public string? SystemName { get; set; }
    }
}
