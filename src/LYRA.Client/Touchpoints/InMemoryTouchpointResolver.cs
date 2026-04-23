using LYRA.Client.Abstractions;
using LYRA.Security.Signing;

namespace LYRA.Client.Touchpoints
{
    /// <summary>
    /// Resolves signing identities from an in-memory list of configurations.
    /// Secret lookup is performed per caller, not per caller-target pair.
    /// </summary>
    public sealed class InMemoryTouchpointResolver : ITouchpointResolver
    {
        private readonly IReadOnlyList<TouchpointConfig> _touchpoints;

        public InMemoryTouchpointResolver(IEnumerable<TouchpointConfig> touchpoints)
        {
            _touchpoints = touchpoints.ToList();
        }

        /// <inheritdoc />
        public TouchpointBinding Resolve(GenericMetadata metadata, string? touchpointKey = null)
        {
            TouchpointConfig? config;

            if (!string.IsNullOrWhiteSpace(touchpointKey))
            {
                config = _touchpoints.FirstOrDefault(t => t.Key == touchpointKey);
            }
            else
            {
                config = _touchpoints.FirstOrDefault(t =>
                    string.Equals(
                        t.CallerSystemName,
                        metadata.CallerSystemName,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (config is null)
                throw new InvalidOperationException(
                    $"Signing identity not found for caller '{metadata.CallerSystemName}' (key: '{touchpointKey ?? "auto"}').");

            return new TouchpointBinding
            {
                Secret = config.Secret,
                SignatureType = config.SignatureType
            };
        }
    }
}
