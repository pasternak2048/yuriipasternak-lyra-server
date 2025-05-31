using LYRA.Server.Entities;

namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Provides access to trusted touchpoint information, typically used for signature validation or authorization checks.
    /// </summary>
    public interface ISecretProvider
    {
        /// <summary>
        /// Retrieves a trusted touchpoint entity by its system name.
        /// </summary>
        /// <param name="systemName">The unique system name of the trusted touchpoint.</param>
        /// <returns>
        /// The corresponding <see cref="TrustedTouchpointEntity"/> if found; otherwise, null.
        /// </returns>
        Task<TrustedTouchpointEntity?> GetTouchpointAsync(string systemName);
    }
}
