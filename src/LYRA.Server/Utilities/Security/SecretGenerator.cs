using System.Security.Cryptography;

namespace LYRA.Server.Utilities.Security
{
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
        /// Generates a secret as a hexadecimal string (optional).
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
