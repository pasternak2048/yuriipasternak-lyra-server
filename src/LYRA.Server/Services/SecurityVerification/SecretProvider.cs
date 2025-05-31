using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Default implementation of <see cref="ISecretProvider"/> that retrieves trusted touchpoint metadata from the database.
    /// </summary>
    public class SecretProvider : ISecretProvider
    {
        private readonly LyraDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretProvider"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The database context used to access touchpoint data.</param>
        public SecretProvider(LyraDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a trusted touchpoint entity by its system name, including its associated company.
        /// Only active and non-deleted touchpoints with active companies are considered.
        /// </summary>
        /// <param name="systemName">The unique system name of the touchpoint.</param>
        /// <returns>
        /// The corresponding <see cref="TrustedTouchpointEntity"/> if found and valid; otherwise, null.
        /// </returns>
        public async Task<TrustedTouchpointEntity?> GetTouchpointAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            return await _context.TrustedTouchpoints
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t =>
                    t.SystemName == systemName &&
                    !t.IsDeleted &&
                    t.IsActive &&
                    t.Company != null &&
                    t.Company.IsActive);
        }
    }
}
