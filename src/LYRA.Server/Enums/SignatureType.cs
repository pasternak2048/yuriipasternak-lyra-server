namespace LYRA.Server.Enums
{
    /// <summary>
    /// Defines the algorithm type used for signing and verifying requests.
    /// </summary>
    public enum SignatureType
    {
        /// <summary>
        /// HMAC (Hash-based Message Authentication Code) signature.
        /// </summary>
        HMAC = 1,

        /// <summary>
        /// RSA asymmetric signature.
        /// </summary>
        RSA = 2,

        /// <summary>
        /// No signature (unsigned).
        /// </summary>
        None = 3
    }
}
