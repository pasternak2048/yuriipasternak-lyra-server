using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.AccessPolicy
{
    /// <summary>
    /// Filter parameters for querying access policies with pagination support.
    /// Inherits common paging properties from <see cref="PaginationFilter"/>.
    /// </summary>
    public class AccessPolicyFilters : PaginationFilter
    {
        /// <summary>
        /// Optional filter by the unique identifier of the caller touchpoint.
        /// </summary>
        public Guid? CallerId { get; set; }

        /// <summary>
        /// Optional filter by the unique identifier of the target touchpoint.
        /// </summary>
        public Guid? TargetId { get; set; }
    }
}
