using LYRA.Server.Enums;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Registry that defines per-context signature behavior (e.g. whether to verify payload hash).
    /// </summary>
    public static class SignatureContextRegistry
    {
        private static readonly Dictionary<AccessContext, SignatureContextMetadata> _map = new()
        {
            [AccessContext.Http] = new SignatureContextMetadata
            {
                Context = AccessContext.Http,
                RequiresPayloadHash = req =>
                {
                    var method = req.Method?.ToUpperInvariant();
                    return method == "POST" || method == "PUT" || method == "PATCH";
                }
            },
            [AccessContext.Event] = new SignatureContextMetadata
            {
                Context = AccessContext.Event,
                RequiresPayloadHash = _ => false
            },
            [AccessContext.Cache] = new SignatureContextMetadata
            {
                Context = AccessContext.Cache,
                RequiresPayloadHash = _ => false
            },
            [AccessContext.Grpc] = new SignatureContextMetadata
            {
                Context = AccessContext.Grpc,
                RequiresPayloadHash = _ => false
            }
        };

        /// <summary>
        /// Gets signature verification metadata for a given access context.
        /// </summary>
        public static SignatureContextMetadata GetMetadata(AccessContext context)
        {
            return _map.TryGetValue(context, out var metadata)
                ? metadata
                : throw new NotSupportedException($"Unsupported AccessContext: {context}");
        }
    }
}
