using System.Security.Cryptography;

namespace LYRA.Server.Utilities.Security
{
    public static class HashHelper
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 64;  // 512 bit
        private const int Iterations = 100_000;

        public static string HashSecret(string secret)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                secret,
                salt,
                Iterations,
                HashAlgorithmName.SHA512,
                KeySize
            );

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifySecret(string input, string stored)
        {
            var parts = stored.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                Iterations,
                HashAlgorithmName.SHA512,
                KeySize
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, storedHash);
        }
    }
}
