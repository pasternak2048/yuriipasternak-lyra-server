namespace LYRA.Server.Models.Company
{
    /// <summary>
    /// Data transfer object returned after successfully creating a company.
    /// Contains company details along with the plaintext secret.
    /// </summary>
    public class CompanyCreatedDto
    {
        /// <summary>
        /// Unique identifier of the company.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Machine-readable system name (slug or code) of the company.
        /// </summary>
        public string SystemName { get; set; } = default!;

        /// <summary>
        /// Human-readable display name of the company.
        /// </summary>
        public string DisplayName { get; set; } = default!;

        /// <summary>
        /// Indicates whether the company is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Timestamp when the company was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The plaintext secret generated for the company (shown only once).
        /// </summary>
        public string SecretPlaintext { get; set; } = default!;
    }
}
