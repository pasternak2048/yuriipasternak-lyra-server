using System.Security.Cryptography;

namespace LYRA.Server.Utilities.Security
{
    /// <summary>
    /// Provides methods to generate cryptographically secure secrets in various formats.
    /// Used for signing keys, authentication tokens, and sensitive system secrets.
    /// </summary>
    public static class SecretGenerator
    {
        /// <summary>
        /// Generates a cryptographically secure random secret.
        /// </summary>
        /// <param name="length">Number of bytes for the secret (default is 32).</param>
        /// <returns>Base64-encoded string of the secret.</returns>
        public static string Generate(int length = 32)
        {
            var bytes = RandomNumberGenerator.GetBytes(length);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Generates a secret as a hexadecimal string.
        /// Useful for non-sensitive display or integrations requiring hex format.
        /// </summary>
        /// <param name="length">Number of bytes for the secret (default is 32).</param>
        /// <returns>Hexadecimal string representation of the secret.</returns>
        public static string GenerateHex(int length = 32)
        {
            var bytes = RandomNumberGenerator.GetBytes(length);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
