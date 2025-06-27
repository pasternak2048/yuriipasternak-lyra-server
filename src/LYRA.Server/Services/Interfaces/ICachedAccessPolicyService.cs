using LYRA.Server.Entities;

namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Service for managing cached access policies in the LyraCachedDbContext.
    /// Used to optimize runtime verification by storing denormalized access data.
    /// </summary>
    public interface ICachedAccessPolicyService
    {
        /// <summary>
        /// Completely replaces all cached access policies with the provided list.
        /// </summary>
        Task ReplaceAllAsync(IEnumerable<CachedAccessPolicyEntity> items);

        /// <summary>
        /// Adds or updates a single cached policy (based on the composite key).
        /// </summary>
        Task UpsertAsync(CachedAccessPolicyEntity item);

        /// <summary>
        /// Deletes a cached policy using Access Policy Id.
        /// </summary>
        Task DeleteByIdAsync(Guid id);

        /// <summary>
        /// Clears all cached access policies.
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// Finds a cached access policy by system names, context, and operation.
        /// </summary>
        Task<CachedAccessPolicyEntity?> FindAsync(string caller, string target, string context, string operation);
    }
}
