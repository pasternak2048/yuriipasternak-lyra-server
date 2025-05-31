using System.Security.Cryptography;
using System.Text;

namespace LYRA.Server.Utilities.Security
{
    /// <summary>
    /// Provides utilities for encryption, decryption, signature generation, and secure comparison.
    /// Used to protect secrets and verify request authenticity.
    /// </summary>
    public static class EncryptionHelper
    {
        // Replace this key with a securely stored and configured key
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");

        /// <summary>
        /// Encrypts the given secret string using AES encryption with a random IV.
        /// The output contains the IV concatenated with the ciphertext, both base64 encoded.
        /// </summary>
        /// <param name="secret">The plaintext secret to encrypt.</param>
        /// <returns>Base64 encoded string containing IV + encrypted secret.</returns>
        public static string EncryptSecret(string secret)
        {
            using var aes = Aes.Create();
            aes.Key = EncryptionKey;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var encryptedBytes = encryptor.TransformFinalBlock(secretBytes, 0, secretBytes.Length);

            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts the base64 encoded string containing IV + encrypted secret.
        /// </summary>
        /// <param name="encryptedSecret">Base64 encoded string containing IV + ciphertext.</param>
        /// <returns>The decrypted plaintext secret.</returns>
        public static string DecryptSecret(string encryptedSecret)
        {
            var fullCipher = Convert.FromBase64String(encryptedSecret);

            using var aes = Aes.Create();
            aes.Key = EncryptionKey;

            var iv = new byte[aes.BlockSize / 8];
            var cipherText = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Computes an HMAC SHA-512 signature for the provided data using the specified key.
        /// </summary>
        /// <param name="data">The input string to sign.</param>
        /// <param name="key">The secret key used for signing.</param>
        /// <returns>Base64 encoded HMAC SHA-512 signature.</returns>
        public static string ComputeHmacSha512(string data, string key)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Performs a time-constant comparison between two strings to prevent timing attacks.
        /// </summary>
        /// <param name="a">First string to compare.</param>
        /// <param name="b">Second string to compare.</param>
        /// <returns>True if strings are equal; otherwise, false.</returns>
        public static bool SecureEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
    }
}
