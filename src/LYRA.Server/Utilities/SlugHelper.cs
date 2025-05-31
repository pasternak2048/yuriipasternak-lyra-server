using System.Text.RegularExpressions;

namespace LYRA.Server.Utilities
{
    /// <summary>
    /// Provides utilities for converting strings into URL-friendly slugs.
    /// Slugs are lowercase, hyphen-separated, and stripped of special characters.
    /// </summary>
    public static class SlugHelper
    {
        /// <summary>
        /// Converts the input string into a slug suitable for URLs or system identifiers.
        /// </summary>
        /// <param name="input">The input string to slugify.</param>
        /// <returns>A lowercase, hyphenated, alphanumeric string without special characters.</returns>
        public static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var lower = input.Trim().ToLowerInvariant();

            var replaced = Regex.Replace(lower, @"[\s_]+", "-");

            var cleaned = Regex.Replace(replaced, @"[^a-z0-9\-]", string.Empty);

            var collapsed = Regex.Replace(cleaned, @"-+", "-");

            return collapsed.Trim('-');
        }
    }
}
