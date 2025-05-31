using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    /// <summary>
    /// Filter model for querying trusted touchpoints with pagination and optional criteria.
    /// </summary>
    public class TrustedTouchpointFilters : PaginationFilter
    {
        /// <summary>
        /// Optional name filter (partial match).
        /// Used to search by system name (case-insensitive).
        /// </summary>
        public string? SystemName { get; set; }

        /// <summary>
        /// Optional filter by owning company ID.
        /// Limits results to touchpoints that belong to the specified company.
        /// </summary>
        public Guid? CompanyId { get; set; }
    }
}
