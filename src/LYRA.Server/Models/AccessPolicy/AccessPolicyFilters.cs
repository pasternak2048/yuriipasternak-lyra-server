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
        /// If provided, only access policies associated with this caller will be returned.
        /// </summary>
        public Guid? CallerId { get; set; }

        /// <summary>
        /// Optional filter by the unique identifier of the target touchpoint.
        /// If provided, only access policies targeting this entity will be returned.
        /// </summary>
        public Guid? TargetId { get; set; }

        /// <summary>
        /// Optional filter by operation string.
        /// For example: "POST /api/verify" or "topic.subscription.created".
        /// Supports partial matches when used in the UI.
        /// </summary>
        public string? Operation { get; set; }
    }
}
