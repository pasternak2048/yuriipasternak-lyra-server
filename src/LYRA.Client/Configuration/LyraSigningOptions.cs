using LYRA.Client.Touchpoints;

namespace LYRA.Client.Configuration
{
    /// <summary>
    /// Options used to configure local signing identities
    /// for outbound requests from this service.
    /// </summary>
    public sealed class LyraSigningOptions
    {
        /// <summary>
        /// Collection of local caller identities used for signing requests.
        /// Each entry defines caller name, secret and algorithm.
        /// Target is not part of secret lookup.
        /// </summary>
        public List<TouchpointConfig> Touchpoints { get; } = new();
    }
}
