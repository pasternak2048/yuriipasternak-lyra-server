namespace LYRA.Server.Utilities
{
    /// <summary>
    /// Utility for generating standardized system-friendly names (slugs) from display names.
    /// Used to create predictable, URL-safe, lowercase identifiers.
    /// </summary>
    public static class NameHelper
    {
        /// <summary>
        /// Converts a display name into a slug format and optionally prefixes it.
        /// </summary>
        /// <param name="displayName">The original human-readable name.</param>
        /// <param name="prefix">Optional prefix to prepend to the slug.</param>
        /// <returns>The generated slug, optionally prefixed.</returns>
        /// <exception cref="ArgumentException">Thrown if displayName is null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the resulting slug is empty.</exception>
        public static string EnsureSlug(string displayName, string? prefix = null)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name must not be empty.");

            var slug = SlugHelper.Slugify(displayName);

            if (string.IsNullOrWhiteSpace(slug))
                throw new InvalidOperationException("Generated name from display name cannot be empty.");

            return string.IsNullOrWhiteSpace(prefix) ? slug : $"{prefix}-{slug}";
        }
    }
}
