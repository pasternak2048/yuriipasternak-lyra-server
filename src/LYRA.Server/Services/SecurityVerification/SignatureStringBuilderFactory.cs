using LYRA.Server.Enums;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Factory responsible for resolving the appropriate <see cref="ISignatureStringBuilder"/>
    /// implementation based on the specified <see cref="AccessContext"/>.
    /// </summary>
    public class SignatureStringBuilderFactory
    {
        private readonly IEnumerable<ISignatureStringBuilder> _builders;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureStringBuilderFactory"/> class with the registered builders.
        /// </summary>
        /// <param name="builders">The collection of available signature string builders.</param>
        public SignatureStringBuilderFactory(IEnumerable<ISignatureStringBuilder> builders)
        {
            _builders = builders;
        }

        /// <summary>
        /// Resolves the appropriate builder for the specified access context.
        /// </summary>
        /// <param name="context">The access context to resolve a builder for (e.g., Http, Event, Cache).</param>
        /// <returns>An instance of <see cref="ISignatureStringBuilder"/> that supports the given context.</returns>
        /// <exception cref="NotSupportedException">Thrown if no builder exists for the given context.</exception>
        public ISignatureStringBuilder GetBuilder(AccessContext context)
        {
            var builder = _builders.FirstOrDefault(b => b.Context == context);
            if (builder == null)
                throw new NotSupportedException($"Unsupported AccessContext: {context}");
            return builder;
        }
    }
}
