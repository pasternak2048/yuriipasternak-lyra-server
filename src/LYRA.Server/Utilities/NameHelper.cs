namespace LYRA.Server.Utilities
{
    public static class NameHelper
    {
        /// <summary>
        /// Normalizes a display name into a slug and validates it is not empty.
        /// Throws if input or result is empty.
        /// </summary>
        public static string NormalizeAndValidate(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name must not be empty.");

            var normalized = SlugHelper.Slugify(displayName);

            if (string.IsNullOrWhiteSpace(normalized))
                throw new InvalidOperationException("Generated name from display name cannot be empty.");

            return normalized;
        }
    }
}
