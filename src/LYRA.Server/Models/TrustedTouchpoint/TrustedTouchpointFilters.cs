using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    public class TrustedTouchpointFilters : PaginationFilter
    {
        /// <summary>
        /// Optional name filter (partial match)
        /// </summary>
        public string? SystemName { get; set; }

        /// <summary>
        /// Optional filter by company ID
        /// </summary>
        public Guid? CompanyId { get; set; }
    }
}
