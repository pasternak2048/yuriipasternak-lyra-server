namespace LYRA.Server.Services.Verify.Interfaces
{
    /// <summary>
    /// Stores recently seen request IDs to prevent replay attacks.
    /// </summary>
    public interface IReplayProtectionStore
    {
        /// <summary>
        /// Marks request ID as used if it was not used before.
        /// Returns false if the request ID already exists.
        /// </summary>
        Task<bool> TryMarkAsUsedAsync(
            string callerSystemName,
            string targetSystemName,
            string requestId,
            TimeSpan ttl,
            CancellationToken ct = default);
    }
}
