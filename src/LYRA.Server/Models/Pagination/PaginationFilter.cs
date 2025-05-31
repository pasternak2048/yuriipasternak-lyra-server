namespace LYRA.Server.Models.Pagination
{
    /// <summary>
    /// Base class for pagination parameters used in filtering queries.
    /// </summary>
    public class PaginationFilter
    {
        /// <summary>
        /// Current page number (1-based). Default is 1.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page. Default is 10.
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
