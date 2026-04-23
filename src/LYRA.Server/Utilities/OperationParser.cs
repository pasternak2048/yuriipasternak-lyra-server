namespace LYRA.Server.Utilities
{
    /// <summary>
    /// Normalizes and matches route rules.
    /// Supports ANY method via "*" and wildcard paths like "/*" or "/api/orders/*".
    /// </summary>
    public static class OperationParser
    {
        public static string NormalizeMethod(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
                return string.Empty;

            var normalized = method.Trim().ToUpperInvariant();

            return normalized == "ANY"
                ? "*"
                : normalized;
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/";

            var normalized = path.Trim();

            if (normalized == "*")
                return "/*";

            if (!normalized.StartsWith('/'))
                normalized = "/" + normalized;

            return normalized.ToLowerInvariant();
        }

        public static bool MethodMatches(string actualMethod, string ruleMethod)
        {
            var actual = NormalizeMethod(actualMethod);
            var rule = NormalizeMethod(ruleMethod);

            return rule == "*"
                   || string.Equals(actual, rule, StringComparison.OrdinalIgnoreCase);
        }

        public static bool PathMatches(string actualPath, string pattern)
        {
            var actual = NormalizePath(actualPath);
            var expected = NormalizePath(pattern);

            if (expected == "/*")
                return true;

            if (expected.EndsWith("/*", StringComparison.Ordinal))
            {
                var prefix = expected[..^1];
                return actual.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
        }
    }
}
