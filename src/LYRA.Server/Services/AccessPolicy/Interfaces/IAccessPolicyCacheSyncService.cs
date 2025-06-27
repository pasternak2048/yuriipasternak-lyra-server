namespace LYRA.Server.Services.AccessPolicy.Interfaces
{
    public interface IAccessPolicyCacheSyncService
    {
        /// <summary>
        /// Synchronizes access policies from the main database into the cache database.
        /// Filters out disabled, inactive, or deleted entries before caching.
        /// </summary>
        Task SyncFromDbAsync();
    }
}
