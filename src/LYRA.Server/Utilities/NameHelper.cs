namespace LYRA.Server.Utilities
{
    public static class NameHelper
    {
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
