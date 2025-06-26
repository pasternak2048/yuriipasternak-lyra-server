using LYRA.Server.Data.LyraDb;
using LYRA.Server.Entities;
using LYRA.Server.Models.TrustedTouchpoint;
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
        /// The corresponding <see cref="TrustedTouchpointInfo"/> if found and valid; otherwise, null.
        /// </returns>
        public async Task<TrustedTouchpointInfo?> GetTouchpointAsync(string normalized)
        {
            return await _context.TrustedTouchpoints
                .AsNoTracking()
                .Where(t =>
                    t.SystemName == normalized &&
                    !t.IsDeleted &&
                    t.IsActive &&
                    t.Company != null &&
                    t.Company.IsActive)
                .Select(t => new TrustedTouchpointInfo
                {
                    Id = t.Id,
                    SystemName = t.SystemName,
                    Secret = t.Secret,
                    UseCompanySecret = t.UseCompanySecret,
                    IsActive = t.IsActive,
                    CompanyName = t.Company!.SystemName,
                    CompanySecret = t.Company.Secret,
                    IsCompanyActive = t.Company.IsActive
                })
                .FirstOrDefaultAsync();
        }
    }
}
