using LYRA.Security.Enums;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.AccessPolicy
{
    /// <summary>
    /// Filter parameters for querying access policies with pagination support.
    /// </summary>
    public class AccessPolicyFilters : PaginationFilter
    {
        /// <summary>
        /// Optional filter by calling touchpoint's system name (partial or exact match).
        /// </summary>
        public string? CallerSystemName { get; set; }

        /// <summary>
        /// Optional filter by target touchpoint's system name (partial or exact match).
        /// </summary>
        public string? TargetSystemName { get; set; }

        /// <summary>
        /// Optional filter by operation identifier (e.g., path, topic).
        /// </summary>
        public string? Operation { get; set; }

        /// <summary>
        /// Optional filter by access context (e.g., Http, Event, Cache).
        /// </summary>
        public AccessContext? Context { get; set; }
    }
}
