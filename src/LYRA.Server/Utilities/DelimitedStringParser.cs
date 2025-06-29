namespace LYRA.Server.Utilities
{
    /// <summary>
    /// Utility class for parsing and serializing delimited string values (e.g., CSV-like).
    /// Commonly used to store and retrieve string arrays (e.g., operations, scopes, tags) as a single string.
    /// </summary>
    public static class DelimitedStringParser
    {
        /// <summary>
        /// Parses a delimited string into an array of trimmed, non-empty string elements.
        /// For example: "get /api/orders , post /api/users" → [ "get /api/orders", "post /api/users" ]
        /// </summary>
        /// <param name="input">The input string to parse (may be null or empty).</param>
        /// <param name="separator">String used to separate values (default: comma).</param>
        /// <returns>An array of non-empty, trimmed strings. Returns an empty array if input is null or whitespace.</returns>
        public static string[] Parse(string? input, string separator = ",")
        {
            return string.IsNullOrWhiteSpace(input)
                ? Array.Empty<string>()
                : input.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        /// <summary>
        /// Serializes a sequence of strings into a single delimited string.
        /// For example: [ "get /api/orders", "post /api/users" ] → "get /api/orders,post /api/users"
        /// </summary>
        /// <param name="items">The collection of strings to join (may be null).</param>
        /// <param name="separator">String used as a delimiter (default: comma).</param>
        /// <returns>A single string with items joined by the separator. Skips null or whitespace-only entries.</returns>
        public static string Join(IEnumerable<string>? items, string separator = ",")
        {
            return items == null
                ? string.Empty
                : string.Join(separator, items.Where(i => !string.IsNullOrWhiteSpace(i)));
        }
    }
}
