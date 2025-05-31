namespace LYRA.Server.Models.Company
{
    /// <summary>
    /// Data transfer object representing a company.
    /// </summary>
    public class CompanyDto
    {
        /// <summary>
        /// Unique identifier of the company.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Machine-readable system name (slug).
        /// </summary>
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Human-readable display name.
        /// </summary>
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// Indicates whether the company is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Timestamp when the company was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
