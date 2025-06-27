namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Universal cache abstraction for storing and retrieving objects by key.
    /// Designed to support memory-based or distributed caches (e.g., Redis).
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a cached item by key.
        /// Returns <c>null</c> if the key does not exist or has expired.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">Unique string key.</param>
        /// <returns>The cached object, or <c>null</c> if not found.</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Adds or updates a cache entry with the specified key and value.
        /// Optionally accepts a time-to-live (TTL) value.
        /// </summary>
        /// <typeparam name="T">The type of the object to store.</typeparam>
        /// <param name="key">Unique string key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="ttl">Optional expiration duration (Time-To-Live).</param>
        Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);

        /// <summary>
        /// Removes a specific cache entry by key.
        /// Safe to call even if the key does not exist.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Clears the entire cache store.
        /// Use with caution — may affect all cached data.
        /// </summary>
        Task ClearAsync();
    }
}
