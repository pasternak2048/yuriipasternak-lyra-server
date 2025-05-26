using System.Security.Cryptography;

namespace LYRA.Server.Utilities.Security
{
    public static class SecretGenerator
    {
        public static string Generate(int length = 32)
        {
            var bytes = RandomNumberGenerator.GetBytes(length);
            return Convert.ToBase64String(bytes);
        }
    }
}
