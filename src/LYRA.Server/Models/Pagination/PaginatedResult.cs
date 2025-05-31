namespace LYRA.Server.Models.Pagination
{
    /// <summary>
    /// Represents a paginated set of results with metadata.
    /// </summary>
    /// <typeparam name="T">Type of the items in the result set.</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// List of items on the current page.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total number of pages available based on total items and page size.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
