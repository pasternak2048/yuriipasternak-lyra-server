using System.Text.RegularExpressions;

namespace LYRA.Server.Utilities
{
    public static class SlugHelper
    {
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
