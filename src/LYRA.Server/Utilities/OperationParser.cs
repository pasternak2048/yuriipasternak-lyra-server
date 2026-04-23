namespace LYRA.Server.Utilities
{
    /// <summary>
    /// Parses policy operation strings in format:
    /// "METHOD /path" or "METHOD /path/*"
    /// </summary>
    public static class OperationParser
    {
        public static (string Method, string PathPattern) ParseSingle(string operation)
        {
            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("Operation is empty.", nameof(operation));

            var parts = operation.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new InvalidOperationException(
                    $"Operation '{operation}' must be in format 'METHOD /path'.");

            return (parts[0].Trim(), NormalizePath(parts[1]));
        }

        public static string NormalizeMethod(string method)
        {
            return string.IsNullOrWhiteSpace(method)
                ? string.Empty
                : method.Trim().ToUpperInvariant();
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/";

            var normalized = path.Trim();

            if (!normalized.StartsWith('/'))
                normalized = "/" + normalized;

            return normalized.ToLowerInvariant();
        }

        public static bool PathMatches(string actualPath, string pattern)
        {
            var actual = NormalizePath(actualPath);
            var expected = NormalizePath(pattern);

            if (expected.EndsWith("/*", StringComparison.Ordinal))
            {
                var prefix = expected[..^1]; // keeps trailing "/"
                return actual.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
        }
    }
}
